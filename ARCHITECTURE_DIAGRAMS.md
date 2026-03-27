# Stock Management - Visual Architecture & Flow Diagrams

## 1. Feature Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                    STOCK MANAGEMENT FEATURE                 │
└─────────────────────────────────────────────────────────────┘

USER INTERFACE LAYER
┌──────────────────────────────────────────┐
│  Views/Stock/Index.cshtml                │
│  - Transaction History                   │
│  - [Add Transaction] Button               │
└──────────────────────────────────────────┘
                    ↑↓
┌──────────────────────────────────────────┐
│  Views/Stock/Create.cshtml               │
│  - Product Dropdown                      │
│  - Type Selection (IN/OUT)               │
│  - Quantity Input                        │
│  - Validation Display                    │
└──────────────────────────────────────────┘

CONTROLLER LAYER
┌──────────────────────────────────────────┐
│  StockController                         │
├──────────────────────────────────────────┤
│  GET Create()                            │
│  ↓ Returns: Products list                │
│                                          │
│  POST Create(StockTransaction)           │
│  ↓ Validates                             │
│  ↓ Calculates stock                      │
│  ↓ Saves changes                         │
│  ↓ Returns: Redirect or Form             │
└──────────────────────────────────────────┘

DATA ACCESS LAYER
┌──────────────────────────────────────────┐
│  ApplicationDbContext                    │
├──────────────────────────────────────────┤
│  DbSet<StockTransaction>                 │
│  DbSet<Product>                          │
└──────────────────────────────────────────┘

DATABASE LAYER
┌──────────────────────────────────────────┐
│  SQL Server Database                     │
├──────────────────────────────────────────┤
│  ┌─ StockTransactions Table             │
│  │  ├─ Id (PK)                          │
│  │  ├─ ProductId (FK → Products)        │
│  │  ├─ Type ('IN' | 'OUT')              │
│  │  ├─ Quantity (int > 0)               │
│  │  └─ CreatedAt (datetime)             │
│  │                                       │
│  └─ Products Table                      │
│     ├─ Id (PK)                          │
│     ├─ Name                             │
│     ├─ Price                            │
│     ├─ Quantity ◄── UPDATED BY THIS    │
│     ├─ LowStockThreshold                │
│     ├─ Description                      │
│     └─ CreatedAt                        │
└──────────────────────────────────────────┘
```

## 2. Stock IN Transaction Flow

```
START
  │
  ↓
┌─────────────────────────────────┐
│  User clicks "Add Transaction"  │
└─────────────────────────────────┘
  │
  ↓
┌─────────────────────────────────┐
│  Form Loads (GET Create)        │
│  - Load all products            │
│  - Display product list         │
└─────────────────────────────────┘
  │
  ↓
┌─────────────────────────────────┐
│  User fills form:               │
│  Product: "Widget A"            │
│  Type: ○ IN  ○ OUT              │
│  Quantity: 100                  │
└─────────────────────────────────┘
  │
  ↓
┌─────────────────────────────────┐
│  User clicks "Save Transaction" │
└─────────────────────────────────┘
  │
  ↓
┌─────────────────────────────────┐
│  POST Create (StockTransaction) │
└─────────────────────────────────┘
  │
  ├─→ ModelState.IsValid? ─→ NO ──→ [Error: Show Form] ─→ STOP
  │
  ↓ YES
┌─────────────────────────────────┐
│  Load Product from DB           │
│  var product = FindById(widget) │
│  Current Stock: 50              │
└─────────────────────────────────┘
  │
  ├─→ Product exists? ─→ NO ──→ [Error: Not Found] ─→ STOP
  │
  ↓ YES
┌─────────────────────────────────┐
│  Type == "IN"?                  │
└─────────────────────────────────┘
  │
  ↓ YES (IN Transaction)
┌─────────────────────────────────┐
│  ADD to inventory:              │
│  product.Quantity += 100        │
│  New Stock: 50 + 100 = 150      │
└─────────────────────────────────┘
  │
  ↓
┌─────────────────────────────────┐
│  Save to Database:              │
│  1. Add StockTransaction        │
│  2. Update Product              │
│  3. Commit changes              │
└─────────────────────────────────┘
  │
  ↓
┌─────────────────────────────────┐
│  SUCCESS                        │
│  ✓ 100 units added              │
│  ✓ Transaction recorded         │
│  ✓ Stock updated                │
└─────────────────────────────────┘
  │
  ↓
┌─────────────────────────────────┐
│  Redirect to Products Index     │
└─────────────────────────────────┘
  │
  ↓
END ✓
```

## 3. Stock OUT Transaction Flow (with Validation)

```
START
  │
  ↓
┌─────────────────────────────────┐
│  User fills form:               │
│  Product: "Widget B"            │
│  Type: ○ IN  ○ OUT              │
│  Quantity: 30                   │
└─────────────────────────────────┘
  │
  ↓
┌─────────────────────────────────┐
│  POST Create (StockTransaction) │
└─────────────────────────────────┘
  │
  ├─→ ModelState.IsValid? ─→ NO ──→ [Error: Show Form] ─→ STOP
  │
  ↓ YES
┌─────────────────────────────────┐
│  Load Product from DB           │
│  Current Stock: 100             │
└─────────────────────────────────┘
  │
  ├─→ Product exists? ─→ NO ──→ [Error: Not Found] ─→ STOP
  │
  ↓ YES
┌─────────────────────────────────┐
│  Type == "OUT"?                 │
└─────────────────────────────────┘
  │
  ↓ YES (OUT Transaction)
┌─────────────────────────────────┐
│  VALIDATE Stock Availability:   │
│  Is 100 >= 30?                  │
└─────────────────────────────────┘
  │
  ├─→ INSUFFICIENT ──→ Error Box:
  │                   "Insufficient stock
  │                    Available: 100
  │                    Requested: 30"
  │   [Reload Form] ──→ STOP
  │
  ↓ SUFFICIENT (100 >= 30 = YES)
┌─────────────────────────────────┐
│  SUBTRACT from inventory:       │
│  product.Quantity -= 30         │
│  New Stock: 100 - 30 = 70       │
└─────────────────────────────────┘
  │
  ↓
┌─────────────────────────────────┐
│  Save to Database:              │
│  1. Add StockTransaction        │
│  2. Update Product              │
│  3. Commit changes              │
└─────────────────────────────────┘
  │
  ↓
┌─────────────────────────────────┐
│  SUCCESS                        │
│  ✓ 30 units removed             │
│  ✓ Transaction recorded         │
│  ✓ Stock updated                │
└─────────────────────────────────┘
  │
  ↓
┌─────────────────────────────────┐
│  Redirect to Products Index     │
└─────────────────────────────────┘
  │
  ↓
END ✓
```

## 4. Form Validation Layers

```
┌─────────────────────────────────────────────────────┐
│              VALIDATION LAYERS                      │
└─────────────────────────────────────────────────────┘

┌─ LAYER 1: HTML5 Client-Side (Browser)
│  ├─ Required attribute
│  ├─ Type="number" min="1" max="10000"
│  └─ Type="radio" required
│
├─ LAYER 2: Razor View Validation
│  ├─ asp-validation-for directives
│  ├─ Validation message spans
│  └─ ModelState checks
│
├─ LAYER 3: Model Data Annotations
│  ├─ [Required]
│  ├─ [Range(1, 10000)]
│  ├─ [RegularExpression("^(IN|OUT)$")]
│  └─ [StringLength]
│
├─ LAYER 4: Controller ModelState
│  ├─ if (!ModelState.IsValid)
│  ├─ ModelState.AddModelError()
│  └─ Form return with errors
│
└─ LAYER 5: Business Logic
   ├─ Product existence check
   ├─ Stock availability check (OUT)
   └─ Quantity calculation
```

## 5. Database Relationship Diagram

```
┌──────────────────────────────┐
│        Products              │
├──────────────────────────────┤
│ Id (PK)                      │
│ Name                         │
│ Price                        │
│ Quantity ◄─── UPDATED        │
│ LowStockThreshold            │
│ Description                  │
│ CreatedAt                    │
└─────────────────┬────────────┘
                  │ (1)
                  │
                  │ One-to-Many
                  │
                  │ (*)
┌─────────────────┴────────────┐
│   StockTransactions          │
├──────────────────────────────┤
│ Id (PK)                      │
│ ProductId (FK) ─────────────→│
│ Type ('IN' | 'OUT')          │
│ Quantity                     │
│ CreatedAt                    │
└──────────────────────────────┘

Relationship Notes:
- 1 Product → Many StockTransactions
- Every transaction linked to 1 product
- Cascade delete possible (optional)
- Product.Quantity updated by transactions
```

## 6. User Interaction Diagram

```
┌──────────────────────────────────────────┐
│         Navigation Menu                  │
│                                          │
│  📦 Dashboard  ← You are here            │
│  📦 Products                             │
│  ↔ Stock ─→ Click                        │
│  🛒 Sales                                │
│  🚚 Purchases                            │
│  👥 Suppliers                            │
│  📊 Reports                              │
└──────────────────────────────────────────┘
                  │
                  ↓
        ┌─────────────────────┐
        │  Stock Index Page   │
        ├─────────────────────┤
        │ ┌─ Add Transaction ─┤ ← Click
        │ │  Button           │
        │ └───────────────────┤
        │                     │
        │ Transaction History │
        │ ┌─────────────────┐ │
        │ │ ID | Type | Qty │ │ ← View list
        │ ├─────────────────┤ │
        │ │ 5  | IN   | 100 │ │
        │ │ 4  | OUT  | -20 │ │
        │ │ 3  | IN   | +50 │ │
        │ └─────────────────┘ │
        └─────────────────────┘
                  │
                  ↓
    ┌─────────────────────────┐
    │  Create Form Page       │
    ├─────────────────────────┤
    │ □ Validation Errors     │ ← If any
    │                         │
    │ Product: [Dropdown ▼]   │
    │ ├─ Widget A (50)        │
    │ ├─ Widget B (100)       │
    │ └─ Widget C (25)        │
    │                         │
    │ Type:                   │
    │ ○ Stock IN (green)      │
    │ ○ Stock OUT (red)       │
    │                         │
    │ Quantity: [100    ]     │
    │                         │
    │ [Save] [Cancel]         │
    └─────────────────────────┘
            │        │
            ↓ Save   ↓ Cancel
         Process   Back to
         & Save    Index


    After Success:
            ↓
    ┌─────────────────────────┐
    │ Products Index (Updated)│
    │ Stock levels changed    │
    │ Return to normal flow   │
    └─────────────────────────┘
```

## 7. State Transitions

```
CREATE FORM STATE MACHINE:

          ┌──────────┐
          │ Loading  │
          └──────┬───┘
                 │
                 ↓
    ┌────────────────────────┐
    │ Form Ready for Input   │ ← User views form
    │ (Empty fields)         │
    └────┬──────────────┬────┘
         │              │
         │ User fills   │ User clicks
         │ form         │ Cancel
         │              │
    ┌────↓─────┐    ┌───↓────────────┐
    │ Submitted │    │ Redirect to    │
    │ (Sending) │    │ Index (Cancel) │
    └────┬─────┘    └────────────────┘
         │
         ├─→ Validation Error ──→ ┌──────────────────┐
         │                        │ Form with Errors │
         │                        │ (User corrects)  │
         │                        └─────┬────────────┘
         │                              │
         │                        ┌─────↓──────────┐
         │                        │ Resubmit Form  │
         │                        └─────┬──────────┘
         │                              │
         │←─────────────────────────────┘
         │
         ├─→ Validation Success
         │   Product exists
         │   Stock available (if OUT)
         │   
         ↓
    ┌─────────────────┐
    │ Save to DB      │
    └────┬────────────┘
         │
         ↓
    ┌──────────────────┐
    │ Success Redirect │
    │ (Products Index) │
    └──────────────────┘
```

## 8. Error Handling Flow

```
POST Create Request
    │
    ├─→ [1] ModelState Invalid?
    │   ├─ Yes → Validation Error Messages
    │   │         Return Form with errors
    │   └─ No → Continue
    │
    ├─→ [2] Product Found?
    │   ├─ No → "Product not found"
    │   │       ModelState.AddModelError()
    │   │       Return Form with error
    │   └─ Yes → Continue
    │
    ├─→ [3] Type == "OUT"?
    │   ├─ Yes → Continue to step [4]
    │   └─ No (IN) → Skip to step [5]
    │
    ├─→ [4] Sufficient Stock?
    │   ├─ No → "Insufficient stock. Available: X"
    │   │       ModelState.AddModelError()
    │   │       Return Form with error
    │   └─ Yes → Continue to step [5]
    │
    ├─→ [5] Calculate New Quantity
    │   ├─ IN → product.Quantity += amount
    │   └─ OUT → product.Quantity -= amount
    │
    ├─→ [6] Save Changes
    │   ├─ StockTransaction added
    │   ├─ Product updated
    │   └─ Database commit
    │
    └─→ [7] Redirect Success
        └─ Redirect to Products Index


Error Types & Locations:
┌──────────────────────────────────────────┐
│ ERROR                │ LOCATION           │
├──────────────────────┼────────────────────┤
│ Required fields      │ Inline (field)     │
│ Invalid type         │ Inline (field)     │
│ Invalid quantity     │ Inline (field)     │
│ Product not found    │ Top of form        │
│ Insufficient stock   │ Top of form        │
│ Database error       │ Top of form        │
└──────────────────────┴────────────────────┘
```

---

This document provides comprehensive visual representation of the Stock Management feature architecture, flows, and interactions.

