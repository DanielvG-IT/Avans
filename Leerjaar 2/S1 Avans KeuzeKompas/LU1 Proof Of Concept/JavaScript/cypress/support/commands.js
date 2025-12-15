// Custom Cypress commands for authentication and navigation
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

// Session-cached login (Cypress 12+).
Cypress.Commands.add('loginSession', (email, password) => {
    cy.session(['auth', email], () => {
        cy.login(email, password);
        // quick sanity check by hitting profile or customer page
        cy.request('/customer').its('status').should('be.oneOf', [200, 302]);
    });
});

// Convenience command to navigate to customer edit page assuming already logged in
Cypress.Commands.add('visitCustomerEdit', () => {
    cy.visit('/customer/edit');
    cy.url().should('include', '/customer/edit');
    cy.get('form#customerForm').should('exist');
});
