# ADR-002: Anonymous Cash Sales

## Status
Proposed - Pending client confirmation.

## Context
The shop frequently sells low-value products to customers who do not want to register their details.

Some anonymous buyers may return products shortly after purchase.

## Decision
- CustomerId is nullable on SaleInvoices.
- Anonymous sales must be fully paid.
- Credit sales require a registered customer.
- Returns reference the original sales invoice.
- Anonymous returns do not create customer accounts.
- Refunds are associated with the original return.

## Consequences
- The shop can sell without collecting customer details.
- Returns can be verified using the invoice number.
- Anonymous debts cannot exist.
- An anonymous invoice cannot be assigned to an existing customer without an explicit controlled operation.
