# StorePilot V2 - Critical Test Scenarios

Implement tests alongside each feature. These are business checks, not a requirement to build a large test framework.

## Reference data and people
- [ ] Units, Groups, and PaymentMethods can be read through the API.
- [ ] No create/update/delete operation is exposed for lookup data.
- [ ] Duplicate lookup names/codes are rejected.
- [ ] Referenced lookup rows cannot be deleted.
- [ ] A person can be a customer, a supplier, or both.

## Products and stock
- [ ] Product requires valid UnitId and GroupId.
- [ ] Duplicate product code is rejected.
- [ ] Opening stock creates a stock movement and correct current quantity.
- [ ] Sale reduces stock; sales return increases it.
- [ ] Purchase increases stock; purchase return decreases it.
- [ ] Negative stock is rejected.
- [ ] Inactive products cannot be added to new documents.

## Purchases and suppliers
- [ ] Immediate purchase is paid in full and increases stock.
- [ ] Credit purchase requires a supplier and increases supplier payable.
- [ ] Purchase return must reference the original invoice item.
- [ ] Repeated returns cannot exceed the purchased quantity.
- [ ] Purchase return reduces supplier payable; excess is recorded as supplier credit.
- [ ] A failed purchase/return leaves no partial stock or account changes.

## Sales and customer accounts
- [ ] Anonymous immediate sale is accepted; anonymous credit sale is rejected.
- [ ] Credit sale without a customer is rejected.
- [ ] Immediate sale preserves its original SaleType.
- [ ] Credit sale increases customer debt.
- [ ] Registered immediate sale and its payment leave the correct net customer balance.
- [ ] Later partial receipt reduces customer debt without selecting an invoice.
- [ ] Receipt above current debt is rejected.
- [ ] Customer balance and statement are derived consistently from ledger entries.

## Sales returns
- [ ] Return must reference the original invoice and item.
- [ ] Return inherits the customer from the original invoice.
- [ ] Anonymous return creates no customer or customer ledger entry.
- [ ] Return quantity cannot exceed the remaining returnable quantity.
- [ ] Return value uses original invoice line values.
- [ ] Return reduces customer debt before refunding excess.
- [ ] Debt 100 / return 30 leaves debt 70 and refunds 0.
- [ ] Debt 10 / return 30 leaves debt 0 and refunds 20.
- [ ] Debt 0 / return 30 refunds 30.
- [ ] Refund cash outflow and stock/account changes are recorded atomically.

## Cash, integrity, and reliability
- [ ] Non-cash payment does not change physical cash.
- [ ] Cash sale/purchase and actual cash refund create the correct cash movement.
- [ ] Posted documents cannot be silently edited or deleted.
- [ ] Repeated posting/return does not duplicate effects.
- [ ] Failure to write stock, ledger, or cash data rolls back the whole operation.
- [ ] Concurrent returns cannot exceed the remaining returnable quantity.

Use unit tests for calculations and integration tests with SQLite for constraints and transaction behavior. Add API tests for validation and response behavior where each endpoint is introduced.
