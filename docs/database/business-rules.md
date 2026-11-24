# StorePilot V2 - Business Rules

## 1. Sales

### BR-SAL-001: Anonymous Cash Sale
- A sale invoice may have no customer.
- An anonymous sale must be fully paid before posting.
- An anonymous sale does not create a customer account entry.

### BR-SAL-002: Registered Customer Sale
- A customer may be selected for either cash or credit sales.
- Selecting a customer is optional for fully paid cash sales.
- A credit sale must have a registered customer.

### BR-SAL-003: Partial Payment
- A registered customer invoice may be partially paid.
- The unpaid amount becomes part of the customer's account balance.
- The amount paid during invoice creation is recorded separately from later debt collections.

### BR-SAL-004: Invoice Posting
- Draft invoices may be edited.
- Posting validates all invoice lines and business rules.
- Posting updates stock and customer account records atomically.
- A posted invoice cannot be silently edited or deleted.
- Repeated posting must not duplicate stock or financial effects.

## 2. Customer Accounts

### BR-CUS-001: Account Balance
Balance = Total Debit - Total Credit.

- Positive: customer owes the shop.
- Zero: account is settled.
- Negative: shop owes the customer; retained credit balances are outside the normal V1 workflow.

### BR-CUS-002: Customer Receipts
- Receipts are independent of specific sales invoices.
- The user selects a customer and enters the received amount.
- The receipt reduces the customer's outstanding balance.
- The amount must not exceed the outstanding balance under the V2 policy.

### BR-CUS-003: Opening Balances
- Existing debts may be entered as opening balances.
- Every opening balance must be represented in the customer ledger.
- Opening balances must be entered through a controlled process.

## 3. Sales Returns

### BR-RET-001: Original Invoice
- A return must reference the original sales invoice.
- Returned items must reference original invoice items.

### BR-RET-002: Returnable Quantity
- Total returned quantity must not exceed the original sold quantity.
- Previously returned quantities must be considered.
- Validation must be performed within a database transaction.

### BR-RET-003: Return Value
- Return value is calculated using the original sale line values and the approved return policy.
- The return reduces outstanding customer debt first.
- Any remaining refundable amount is returned to the customer.
- V2 does not automatically retain return value as future customer credit.

### BR-RET-004: Anonymous Return
- An anonymous return is identified through the original invoice.
- It does not create a customer record or customer account entry.
- Any refund is recorded against the return.

## 4. Inventory

### BR-STK-001
- Posted sales decrease stock.
- Posted returns increase stock.
- Stock movement and current quantity updates must be atomic.

### BR-STK-002
- Inactive products cannot be used in new sales.
- Historical documents must remain readable after product changes.

### BR-STK-003
- Negative stock is not allowed unless explicitly approved as a business rule.
- The initial policy should reject sales exceeding available stock.

## 5. Cash

### BR-CASH-001
- Cash transactions represent actual physical cash movements.
- Cash received during sales and debt collection increases cash.
- Cash refunds decrease cash.
- A non-cash payment must not be recorded as physical cash.

## 6. Data Integrity

- Posted documents are immutable except through controlled reversal/cancellation.
- All related document, stock, cash, and account changes must be atomic.
- Foreign keys protect historical relationships.
- Important constraints must be enforced in both application logic and database schema where possible.
- All financial values must follow one defined currency precision and rounding policy.

## 7. Decisions Requiring Client Confirmation

- Whether returns without an original invoice are allowed.
- Whether refunds must use the original payment method.
- Whether the shop uses bank transfers or cards in V1.
- Whether old customer balances must be imported.
- Whether quotations are mandatory in V2.
- Whether the customer needs per-invoice outstanding balances.
