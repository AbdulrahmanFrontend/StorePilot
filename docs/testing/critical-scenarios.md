# StorePilot V2 - Critical Test Scenarios

## Sales

- [ ] Anonymous cash sale is accepted.
- [ ] Anonymous unpaid or partially paid sale is rejected.
- [ ] Credit sale without a customer is rejected.
- [ ] Registered customer cash sale is accepted.
- [ ] Registered customer partial-payment sale is accepted.
- [ ] Invoice totals are calculated correctly.
- [ ] Posting the same invoice twice does not duplicate effects.
- [ ] Failed posting leaves no partial stock or account changes.

## Customer Accounts

- [ ] Credit sale increases customer debit balance.
- [ ] Payment during invoice creation decreases customer balance.
- [ ] Later customer receipt decreases customer balance.
- [ ] Partial receipt leaves the correct outstanding balance.
- [ ] Receipt above outstanding balance is rejected under V1 policy.
- [ ] Opening balance appears correctly in the account statement.
- [ ] Customer statement ordering is deterministic.

## Returns

- [ ] Return linked to an original invoice is accepted.
- [ ] Return quantity cannot exceed remaining returnable quantity.
- [ ] Repeated return cannot exceed original sold quantity.
- [ ] Anonymous return does not create a customer account entry.
- [ ] Return increases stock.
- [ ] Refund decreases cash.
- [ ] Return amount is applied to customer debt before refunding any excess.
- [ ] Failed return leaves no partial stock, cash, or account changes.

## Inventory

- [ ] Sale decreases current quantity.
- [ ] Return increases current quantity.
- [ ] Sale exceeding available stock is rejected.
- [ ] Inactive product cannot be used in a new sale.
- [ ] Product quantity is consistent with stock transactions.

## Data Integrity

- [ ] Duplicate product code is rejected.
- [ ] Duplicate unit name is rejected.
- [ ] Duplicate group name is rejected.
- [ ] Referenced units cannot be deleted.
- [ ] Referenced groups cannot be deleted.
- [ ] Historical product data remains unchanged after product edits.
- [ ] Posted documents cannot be silently edited or deleted.

## Concurrency and Atomicity

- [ ] Two concurrent returns cannot exceed the remaining returnable quantity.
- [ ] Two concurrent postings cannot duplicate document effects.
- [ ] Failure to create a stock transaction rolls back the posting.
- [ ] Failure to create a customer ledger entry rolls back the posting.
- [ ] Failure to create a cash transaction rolls back the related payment operation.

## Test Strategy

- Unit tests for calculations and isolated business rules.
- Integration tests for database constraints and transaction behavior.
- API tests for validation, status codes, and end-to-end use cases.
- SQLite test database configured with the same important constraints as production.
