# StorePilot V2 - Product Scope

## 1. Goal and deployment

StorePilot V2 is a local management system for one shop:
- ASP.NET Core API, React + TypeScript UI, and one local SQLite database.
- Desktop hosting through WebView is planned.
- No cloud sync, branches, warehouses, user permissions, or full accounting in this version.

Deliver the shop's daily workflows in small, testable steps. Do not build abstractions or reports before a real workflow needs them.

## 2. V2 implementation order

1. **Reference data:** make Units and Groups read-only lookup data; add read-only PaymentMethods and seed the values actually used by the shop. Remove management actions that no longer have a purpose.
2. **People and roles:** retain People; add Customer and Supplier roles. A person may have either or both roles.
3. **Products:** catalog, prices, units/groups, active state, and current stock.
4. **Opening stock and inventory history:** record opening quantities and stock movements; do not allow negative stock.
5. **Purchases and suppliers (simple first version):** record purchase invoices and items to increase stock; support cash purchases and credit purchases. Credit purchases require a supplier and create a supplier balance. Add purchase returns linked to original purchase items.
6. **Customers and sales:** anonymous fully-paid sales, registered customer cash sales, and credit sales.
7. **Customer accounts:** independent receipts and customer statements; no invoice allocation or FIFO.
8. **Sales returns:** link to original sale items, restore stock, offset customer debt first, and refund any excess.
9. **Supplier payments and cash overview:** add independent supplier debt payments after purchase and return workflows are stable.
10. **Dashboard and quotations:** replace placeholder/reference-data counts with useful operational metrics once real sales and stock data exist. Add saved quotations only if the shop confirms they are needed.

The order is intentional: Units and Groups are the next feature, followed by People/Customers and Products. Purchase foundations are included in V2 so stock can be replenished after opening quantities; supplier debt settlement can follow later.

## 3. Included workflows

### Reference data
- Units, Groups, and PaymentMethods are database lookup tables.
- They are read-only in the UI and API; no create, edit, or delete endpoints.
- Seed shop-required values. Do not hard-code shop-specific units or groups as enums.
- Do not delete a lookup value referenced by historical data.

### People
- People stores shared name and optional contact details.
- Customers and Suppliers identify the person's role.
- A person may be both a customer and supplier.
- Do not require unique names or phone numbers unless the actual shop workflow proves that necessary.

### Sales and customer accounts
- Anonymous sales are fully paid and have no customer account.
- Credit sales require a registered customer.
- A registered customer may also make a fully paid sale.
- Payments made at sale time are recorded with that sale; later debt collections are independent receipts and do not select an invoice.
- No partial payment during sale creation in the first workflow; later receipts may be partial.
- Customer balance is derived from its account entries, not stored as an independently editable total.

### Returns
- Every return line references its original invoice item.
- One return may combine items from several invoices belonging to the same registered customer, or several anonymous invoices; do not mix different customers or anonymous and registered invoices.
- The original invoice determines customer identity; do not create a customer for an anonymous return.
- A registered customer's return reduces their outstanding debt first. Any amount left after the debt reaches zero is refunded immediately.
- If the customer has no debt, refund the full return value immediately.
- An anonymous return is verified using the original invoice and refunded; no customer is created.
- Do not allow returned quantity to exceed the remaining returnable quantity for each original invoice item.

### Purchases and suppliers
- Purchase invoices and their items increase stock.
- A credit purchase requires a supplier and increases the amount owed to that supplier.
- A cash purchase is paid in full at entry in the initial workflow.
- Purchase returns reference the original purchase invoice items, restore the supplier account appropriately, and increase/decrease stock as applicable.
- Supplier debt payments are a later step; do not block recording purchases while that feature is pending.
- Supplier refunds and complex supplier-credit settlement are not part of the first purchasing step.

## 4. Deferred or conditional

- Supplier payment and advanced supplier account settlement.
- Per-invoice customer payment allocation, FIFO, and invoice-level outstanding balances.
- Multiple warehouses/branches and cloud synchronization.
- User accounts, permissions, and advanced financial accounts.
- Full accounting, tax, and advanced analytics.
- Saved quotations unless the shop confirms a need; a print preview without saving may be enough.
- Complex purchase order workflows and supplier price lists.

## 5. Delivery rules

- Keep posted documents and their history; correct them through a controlled return, cancellation, or reversal.
- Commit document, stock, account, and cash effects in one database transaction.
- Use EF Core migrations for schema changes.
- Add tests for business rules and database constraints before expanding the UI.
- Do not redesign the dashboard around guessed metrics. Remove reference-data counts when updating it and add useful operational metrics after sales and inventory workflows exist.
