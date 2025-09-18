// ***********************************************
// This example commands.js shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************
//
//
// -- This is a parent command --
// Cypress.Commands.add('login', (email, password) => { ... })
// Programmatic login hitting the backend API to obtain auth cookies
// Usage: cy.login('user@example.com','Password123!')
Cypress.Commands.add('login', (email, password) => {
    if (!email || !password) throw new Error('cy.login requires email and password');
    return cy
        .request({
            method: 'POST',
            url: '/auth/login',
            body: { email, password },
            failOnStatusCode: false,
        })
        .then((resp) => {
            expect(resp.status, `login status for ${email}`).to.eq(200);
            // Cookies are set via httpOnly in response; Cypress will store them automatically.
        });
});

// Convenience command to navigate to customer edit page assuming already logged in
Cypress.Commands.add('visitCustomerEdit', () => {
    cy.visit('/customer/edit');
    cy.url().should('include', '/customer/edit');
    cy.get('form#customerForm').should('exist');
});
//
//
// -- This is a child command --
// Cypress.Commands.add('drag', { prevSubject: 'element'}, (subject, options) => { ... })
//
//
// -- This is a dual command --
// Cypress.Commands.add('dismiss', { prevSubject: 'optional'}, (subject, options) => { ... })
//
//
// -- This will overwrite an existing command --
// Cypress.Commands.overwrite('visit', (originalFn, url, options) => { ... })
