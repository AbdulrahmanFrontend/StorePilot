# ADR-003: Reference Data in V2

## Status
Proposed.

## Decision

### Units
- Store as database lookup data.
- Seed initial values.
- No Active, CreatedAt, or Notes fields.
- Do not delete units referenced by products.

### Groups
- Store as database lookup data.
- Initial data is shop-specific.
- No Active, CreatedAt, or Notes fields in V1.
- Do not delete groups referenced by products.

### PaymentMethods
- If cash is the only supported method, defer the table.
- If multiple methods are required, use seeded lookup records.
- No separate CRUD interface for fixed methods.
- Do not delete methods referenced by historical payments.

## Consequences
- Smaller schema and simpler management.
- Adding dynamic lookup management later may require a migration.
- Inactive lookup management is not supported in V1.
