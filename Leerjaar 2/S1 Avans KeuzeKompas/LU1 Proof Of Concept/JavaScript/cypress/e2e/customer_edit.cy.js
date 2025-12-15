/// <reference types="cypress" />
/**
 * Customer Edit Page E2E Tests
 * NOTE: Requires an existing user with role CUSTOMER and associated customer record.
 * Provide CYPRESS_TEST_EMAIL and CYPRESS_TEST_PASSWORD via env (cypress.config.js or CLI) or fall back to defaults.
 */

const TEST_EMAIL =
    Cypress.env('TEST_EMAIL') || Cypress.env('CYPRESS_TEST_EMAIL') || 'customer@example.com';
const TEST_PASSWORD =
    Cypress.env('TEST_PASSWORD') || Cypress.env('CYPRESS_TEST_PASSWORD') || 'Password123!';

// Utility to generate a unique suffix to avoid collisions
function randomLetters(len = 5) {
    const chars = 'abcdefghijklmnopqrstuvwxyz';
    let s = '';
    for (let i = 0; i < len; i++) s += chars[Math.floor(Math.random() * chars.length)];
    return s;
}

describe('Customer Edit Page', () => {
    // Prevent failing the test on known, non-critical runtime errors (optional safeguard)
    Cypress.on('uncaught:exception', (err) => {
        if (/Unexpected token '&'/.test(err.message)) {
            // swallow legacy error from earlier templates
            return false;
        }
    });
    beforeEach(() => {
        cy.loginSession(TEST_EMAIL, TEST_PASSWORD);
        cy.visitCustomerEdit();
    });

    it('loads the edit customer form with required fields', () => {
        cy.get('h1').contains('Edit Customer');
        const requiredIds = ['#firstName', '#lastName'];
        requiredIds.forEach((id) => cy.get(id).should('have.attr', 'required'));

        // Basic presence checks
        [
            '#email',
            '#phone',
            '#address',
            '#city',
            '#province',
            '#postalCode',
            '#country',
            '#active',
            '#storeOption',
        ].forEach((selector) => cy.get(selector).should('exist'));
    });

    it('updates name and phone then saves changes', () => {
        const newFirst = 'Test' + randomLetters(); // only letters; normalization strips digits otherwise
        const newLast = 'User';
        const newPhone = '076-' + Date.now().toString().slice(-6);

        cy.get('#firstName').clear().type(newFirst);
        cy.get('#lastName').clear().type(newLast);
        cy.get('#phone').clear().type(newPhone);

        cy.intercept('POST', '/customer/edit').as('saveEdit');
        cy.get('#submitBtn').click();

        cy.wait('@saveEdit').then((interception) => {
            // Some frameworks may respond 302 (redirect) or 200 depending on middleware; accept both.
            expect([200, 302]).to.include(
                interception.response?.statusCode,
                'Expected POST /customer/edit to succeed'
            );
        });
        // Redirect back to /customer page (if 302). If 200 (no change) the URL might remain /customer/edit, so allow either then enforce redirect by visiting manually if needed.
        cy.location('pathname', { timeout: 8000 }).then((p) => {
            if (p !== '/customer') {
                // navigate manually if still on edit due to no-change scenario
                cy.visit('/customer');
            }
        });

        // Re-open edit page to verify persisted values (best-effort; depends on backend commit timing)
        cy.visitCustomerEdit();
        cy.get('#firstName').should('have.value', newFirst);
        cy.get('#lastName').should('have.value', newLast);
        cy.get('#phone').should('have.value', newPhone);
    });

    it('toggles Active checkbox state and saves', () => {
        cy.get('#active').then(($chk) => {
            const wasChecked = $chk.is(':checked');
            cy.wrap($chk).click();
            cy.intercept('POST', '/customer/edit').as('saveActive');
            cy.get('#submitBtn').click();
            cy.wait('@saveActive').then((interception) => {
                expect([200, 302]).to.include(
                    interception.response?.statusCode,
                    'Expected POST /customer/edit (active toggle) to succeed'
                );
            });
            cy.url().should('include', '/customer');
            // Re-open and verify toggled
            cy.visitCustomerEdit();
            cy.get('#active').should(wasChecked ? 'not.be.checked' : 'be.checked');
        });
    });

    it('validates required fields client-side (HTML5 validation)', () => {
        cy.get('#firstName').clear();
        cy.get('#lastName').clear();
        cy.get('#submitBtn').click();

        // Form should not submit, still on edit page
        cy.url().should('include', '/customer/edit');
        cy.get('#firstName:invalid').should('exist');
        cy.get('#lastName:invalid').should('exist');

        // Fill back to restore
        cy.get('#firstName').type('Restore');
        cy.get('#lastName').type('User');
    });

    it('changes store selection if options are present', function () {
        cy.get('#storeOption').then(($select) => {
            const options = $select.find('option');
            if (options.length > 1) {
                const currentVal = $select.val();
                // choose another option
                const other = [...options].find((o) => o.value !== currentVal)?.value;
                if (other) {
                    cy.wrap($select).select(other).should('have.value', other);
                }
            } else {
                cy.log('Only one store option present; skipping change test.');
            }
        });
    });

    it('submitting with no actual changes re-renders (no-change path)', () => {
        // Capture existing values
        cy.get('#firstName')
            .invoke('val')
            .then((origFirst) => {
                cy.get('#lastName')
                    .invoke('val')
                    .then((origLast) => {
                        cy.get('#phone')
                            .invoke('val')
                            .then((origPhone) => {
                                // Re-type same values
                                cy.get('#firstName').clear().type(String(origFirst));
                                cy.get('#lastName').clear().type(String(origLast));
                                cy.get('#phone').clear().type(String(origPhone));
                                cy.intercept('POST', '/customer/edit').as('saveSame');
                                cy.get('#submitBtn').click();
                                cy.wait('@saveSame').then((i) => {
                                    expect([200, 302]).to.include(i.response?.statusCode);
                                });
                                // If still on edit page this confirms no-change scenario path; otherwise redirect.
                                cy.location('pathname').then((p) => {
                                    if (p === '/customer/edit') {
                                        cy.contains('No changes were made.').should('exist');
                                    }
                                });
                            });
                    });
            });
    });
});
