import cron from 'node-cron';
import { logger } from '../util/logger.js';
import { sendEmail } from '../services/mailService.js';
import { readOverdueRentals, markOverdueEmailSent } from '../dao/rental.js';

logger.info('Overdue worker started. Scheduling daily job at 3 AM.');

// Run every minute for testing; change to '0 3 * * *' for daily at 3 AM
cron.schedule('* * * * *', () => {
    readOverdueRentals((err, overdue) => {
        if (err) {
            logger.error('[CRON] Error fetching overdue rentals:', err);
            return;
        }

        if (!overdue || overdue.length === 0) {
            logger.info('[CRON] No overdue rentals found.');
            return;
        }

        const TWO_DAYS_MS = 2 * 24 * 60 * 60 * 1000;
        const now = new Date();
        let remaining = 0;
        let sentCount = 0;

        overdue.forEach((r) => {
            const lastSent = r.overdue_mail_sent_at ? new Date(r.overdue_mail_sent_at) : null;
            const shouldSend = !lastSent || now - lastSent > TWO_DAYS_MS;

            if (!shouldSend) return; // skip if recently sent

            remaining++;
            const esc = (s) =>
                String(s ?? '')
                    .replace(/&/g, '&amp;')
                    .replace(/</g, '&lt;')
                    .replace(/>/g, '&gt;')
                    .replace(/"/g, '&quot;')
                    .replace(/'/g, '&#39;');

            const dueDate = r.due_date
                ? new Date(r.due_date).toISOString().slice(0, 10)
                : 'Unknown';

            const html = `<!doctype html>
<html>
<head>
  <meta charset="utf-8" />
  <title>Overdue Movie Rental</title>
</head>
<body style="margin:0;padding:0;background:#f5f7fa;font-family:Helvetica,Arial,sans-serif;color:#333;">
  <table role="presentation" width="100%" cellpadding="0" cellspacing="0">
    <tr>
      <td align="center" style="padding:24px;">
        <table role="presentation" width="600" cellpadding="0" cellspacing="0" style="background:#ffffff;border-radius:8px;overflow:hidden;box-shadow:0 4px 20px rgba(16,24,40,0.08);">
          <tr>
            <td style="padding:20px 24px;border-bottom:1px solid #eef2f7;">
              <h1 style="margin:0;font-size:20px;color:#0f172a;">Overdue Movie Rental</h1>
              <p style="margin:8px 0 0;color:#64748b;font-size:13px;">Friendly reminder about a rental that is past due.</p>
            </td>
          </tr>
          <tr>
            <td style="padding:24px;">
              <p style="margin:0 0 12px;font-size:16px;">Hi ${esc(r.first_name)},</p>
              <p style="margin:0 0 16px;font-size:14px;line-height:1.5;color:#334155;">
                Our records show your rental for <strong>${esc(
                    r.title
                )}</strong> was due on <strong>${dueDate}</strong>.
                Please return it as soon as possible to avoid late fees.
              </p>
              <p style="margin:0 0 8px;">
                <a href="mailto:support@example.com?subject=${encodeURIComponent(
                    'Return: ' + (r.title || 'rental')
                )}" style="display:inline-block;padding:10px 16px;background:#2563eb;color:#ffffff;border-radius:6px;text-decoration:none;font-weight:600;font-size:14px;">
                  Reply to arrange return
                </a>
              </p>
            </td>
          </tr>
          <tr>
            <td style="padding:16px 24px;background:#f8fafc;border-top:1px solid #eef2f7;font-size:12px;color:#64748b;">
              <div>Rental ID: ${esc(r.rental_id)}</div>
              <div style="margin-top:6px;">If you believe this is an error, contact <a href="mailto:support@eject.nl">support@eject.nl</a>.</div>
            </td>
          </tr>
        </table>
        <div style="font-size:12px;color:#94a3b8;margin-top:12px;">&copy; ${new Date().getFullYear()} Movie Rentals</div>
      </td>
    </tr>
  </table>
</body>
</html>`;

            sendEmail(r.email, 'Overdue Movie Rental', html, (mailErr) => {
                if (mailErr) {
                    logger.error('[CRON] Error sending email to', r.email, mailErr);
                } else {
                    sentCount++;
                    // Update DB to mark email sent
                    markOverdueEmailSent(r.rental_id, (updateErr) => {
                        if (updateErr) {
                            logger.error(
                                '[CRON] Failed to update overdue_mail_sent_at for',
                                r.rental_id,
                                updateErr
                            );
                        }
                    });
                }

                remaining--;
                if (remaining === 0) {
                    logger.info(`[CRON] Sent ${sentCount} overdue emails.`);
                }
            });
        });

        if (remaining === 0) {
            logger.info('[CRON] No overdue emails needed sending today.');
        }
    });
});
