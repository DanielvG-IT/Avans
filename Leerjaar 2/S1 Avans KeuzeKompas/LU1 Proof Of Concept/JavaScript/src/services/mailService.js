import nodemailer from 'nodemailer';

const transporter = nodemailer.createTransport({
    host: process.env.SMTP_HOST,
    port: process.env.SMTP_PORT,
    secure: false,
    auth: {
        user: process.env.SMTP_USER,
        pass: process.env.SMTP_PASS,
    },
});

export const sendEmail = (to, subject, html, callback) => {
    return transporter.sendMail(
        {
            from: '"Eject! Movie Rental" <noreply@eject.avans.nl>',
            to,
            subject,
            html,
        },
        callback
    );
};
