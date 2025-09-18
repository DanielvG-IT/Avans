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
function uniqueSuffix() {
    return Date.now().toString().slice(-6);
}

describe('Customer Edit Page', () => {
    beforeEach(() => {
        cy.login(TEST_EMAIL, TEST_PASSWORD);
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
        const newFirst = 'Test' + uniqueSuffix();
        const newLast = 'User';
        const newPhone = '555-1' + uniqueSuffix();

        cy.get('#firstName').clear().type(newFirst);
        cy.get('#lastName').clear().type(newLast);
        cy.get('#phone').clear().type(newPhone);

        cy.intercept('POST', '/customer/edit').as('saveEdit');
        cy.get('#submitBtn').click();

        // Redirect back to /customer page
        cy.url({ timeout: 10000 }).should('include', '/customer');
        cy.wait('@saveEdit').its('response.statusCode').should('eq', 302);

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
            cy.wait('@saveActive');
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
});
