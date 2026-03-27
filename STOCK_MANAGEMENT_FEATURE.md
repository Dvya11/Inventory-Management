# Stock Management Feature Documentation

## Overview
The Stock Management feature provides a complete solution for tracking inventory transactions (stock IN and OUT) in the ASP.NET Core MVC Inventory Management System.

## Components

### 1. Model: StockTransaction
**Location:** `Models/StockTransaction.cs`

**Properties:**
- `Id` (int) - Primary key, auto-incremented
- `ProductId` (int) - Foreign key to Products table
- `Type` (string) - Transaction type ("IN" or "OUT")
- `Quantity` (int) - Amount of stock added or removed (minimum 1)
- `CreatedAt` (DateTime) - Timestamp of transaction (defaults to current date/time)
- `Product` (Navigation) - Navigation property to Product

**Validation Rules:**
- All fields are required
- Type must match regex: `^(IN|OUT)$` (only "IN" or "OUT" allowed)
- Quantity must be between 1 and 10,000

### 2. Controller: StockController
**Location:** `Controllers/StockController.cs`

#### GET: Create()
Loads the form to create a new stock transaction.
- Fetches all products from database
- Populates dropdown with product list and current stock levels
- Returns Create view with products and transaction types

#### POST: Create(StockTransaction model)
Handles stock transaction submission with validation and logic:

**Validation:**
- ModelState is checked for all required fields
- Product must exist in database
- Quantity must be positive integer

**Business Logic:**
- **If Type == "IN"**: Adds quantity to product's stock
  - `product.Quantity += model.Quantity`
  
- **If Type == "OUT"**: Validates sufficient stock exists
  - Checks: `product.Quantity >= model.Quantity`
  - If insufficient: Returns error "Insufficient stock. Available: X, Requested: Y"
  - If sufficient: `product.Quantity -= model.Quantity`

**Data Persistence:**
- Saves StockTransaction record to database
- Updates Product quantity in database
- Commits both changes atomically
- Redirects to Product Index on success

### 3. Views

#### Index View (`Views/Stock/Index.cshtml`)
Displays all stock transactions in chronological order.

**Features:**
- Table with columns: Transaction ID, Product Details, Type, Quantity, Date & Time
- Type badge styling:
  - **IN**: Green background with down arrow icon
  - **OUT**: Red background with up arrow icon
- Quantity color-coded:
  - **IN**: Green with "+" prefix
  - **OUT**: Red with "-" prefix
- "Add Transaction" button links to Create page
- Empty state message when no transactions exist
- Search functionality (placeholder)

#### Create Form (`Views/Stock/Create.cshtml`)
Professional form for adding stock transactions.

**Form Fields:**
1. **Product Selector**
   - Dropdown with all available products
   - Shows current stock level next to each product name
   - Format: "Product Name (Stock: 123)"
   - Validation message on error

2. **Transaction Type**
   - Radio buttons for "IN" and "OUT"
   - Color-coded styling:
     - IN: Green background with down arrow
     - OUT: Red background with up arrow
   - Visually distinct selection with hover effects

3. **Quantity Input**
   - Number input field
   - Minimum value: 1
   - Placeholder text: "Enter quantity"
   - Validation message on error

**Validation Display:**
- Shows all validation errors at top of form
- Inline validation messages under each field
- Error states highlight problematic inputs in red

**Styling:**
- Professional card-based layout
- Consistent with product and other pages
- Bootstrap-free (custom CSS)
- Color scheme:
  - Primary color: #E11D48 (rose)
  - Success: #01B574 (green)
  - Danger: #EE5D50 (red)
- Responsive design

### 4. Database Migration
**File:** `Migrations/20260327131716_AddStockTransactions.cs`

Creates the `StockTransactions` table with:
- Primary key on `Id`
- Foreign key constraint on `ProductId` → `Products.Id`
- Varchar(1000) for Type field
- DateTime column for CreatedAt
- Integer column for Quantity

## Usage Flow

### Adding Stock (IN Transaction)
1. Navigate to Stock Management → Add Transaction
2. Select product from dropdown
3. Select "Stock IN" option
4. Enter quantity to add
5. Click "Save Transaction"
6. System adds quantity to product stock
7. Transaction record created in database
8. Redirected to Products list

### Removing Stock (OUT Transaction)
1. Navigate to Stock Management → Add Transaction
2. Select product from dropdown
3. Select "Stock OUT" option
4. Enter quantity to remove
5. Click "Save Transaction"
6. System validates stock availability:
   - If insufficient: Error message displayed, form reloaded
   - If available: Quantity subtracted from product stock
7. Transaction record created in database
8. Redirected to Products list

### Viewing History
1. Navigate to Stock Management
2. View all transactions in table
3. Identify transaction type by badge (IN/OUT)
4. See quantity change with color coding (green/red)
5. Check timestamp of each transaction

## Error Handling

**Insufficient Stock Error:**
- Message: "Insufficient stock. Available: X, Requested: Y"
- Appears when OUT transaction exceeds available stock
- Form is preserved with user input
- User can modify quantity and resubmit

**Invalid Product:**
- Message: "Product not found"
- Appears if selected product doesn't exist
- Returns user to form with error

**Validation Errors:**
- Type must be IN or OUT
- Quantity must be 1 or greater
- All fields required
- Messages appear both at form top and under fields

## Database Schema

```sql
CREATE TABLE [StockTransactions] (
    [Id] int NOT NULL IDENTITY,
    [ProductId] int NOT NULL,
    [Type] nvarchar(1000) NOT NULL,
    [Quantity] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_StockTransactions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StockTransactions_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id])
);
```

## Integration Points

### Product Model
- One-to-Many relationship: Product → StockTransactions
- Navigation property: `List<StockTransaction> StockTransactions`

### ApplicationDbContext
- DbSet: `public DbSet<StockTransaction> StockTransactions { get; set; }`
- Automatically configured by Entity Framework Core

### Dashboard/Navigation
- Stock Management link in sidebar
- Icon: Arrow right-arrow-left
- Controller: Stock, Action: Index

## Best Practices Implemented

1. **Data Validation:**
   - Model-level validation with annotations
   - Server-side validation in controller
   - Client-side validation in view

2. **Error Handling:**
   - Graceful error messages
   - ModelState preservation for form resubmission
   - Transaction logging (all changes recorded)

3. **User Experience:**
   - Color-coded transaction types
   - Clear visual hierarchy
   - Intuitive form layout
   - Search and filter capabilities

4. **Database Integrity:**
   - Foreign key constraints
   - Atomic transactions (both records saved together)
   - Timestamp tracking for auditing

5. **Security:**
   - Anti-forgery token validation
   - Input validation
   - Type constraints (IN/OUT only)

## Performance Considerations

- Index view uses `Include()` to eager-load related products
- Transactions ordered by CreatedAt descending (most recent first)
- Indexes recommended on ProductId and CreatedAt columns

## Future Enhancements

- Batch stock transactions
- Stock adjustment approvals
- Reason/notes field for transactions
- Stock movement reports
- Low stock alerts integration
- Barcode scanning for products
