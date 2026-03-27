# Stock Management Feature - Quick Start Guide

## 🚀 Feature Overview

The Stock Management feature allows you to track and manage inventory transactions with two simple transaction types:

- **IN** ✅ (Green) - Stock received/added
- **OUT** ❌ (Red) - Stock issued/removed

## 📍 Accessing the Feature

### Via Navigation
1. Open the application
2. Look for **Stock** in the left sidebar (with arrow icon ↔)
3. Click to go to Stock Management page

### From Products Page
- After managing products, use Stock Management to track changes

## 📋 Creating a Stock Transaction

### Step 1: Click "Add Transaction"
- Located at top-right of Stock Management page
- Or navigate directly to `/Stock/Create`

### Step 2: Fill the Form

**Product Selection:**
```
[Dropdown showing all products with current stock]
Example: "Widget A (Stock: 50)" ✓
```

**Transaction Type:**
```
○ Stock IN   (add inventory)
○ Stock OUT  (remove inventory)
```

**Quantity:**
```
[Number Input]
Enter: 25  (minimum 1, maximum 10,000)
```

### Step 3: Submit
- Click **"Save Transaction"** button
- System validates and processes
- Redirected to Products page on success

## ✔️ Successful Transaction

**Stock IN Example:**
```
Product: "Widget A"
Current Stock: 50
Transaction: +25 (IN)
New Stock: 75 ✓
```

**Stock OUT Example:**
```
Product: "Widget A"
Current Stock: 75
Transaction: -20 (OUT)
New Stock: 55 ✓
```

## ⚠️ Error Scenarios

### Insufficient Stock Error
```
Product: "Widget A"
Current Stock: 50
Attempted OUT: 100 units
Result: ❌ ERROR
Message: "Insufficient stock. Available: 50, Requested: 100"
Action: Modify quantity and resubmit
```

### Validation Errors
| Error | Solution |
|-------|----------|
| No product selected | Choose a product from dropdown |
| No type selected | Select either IN or OUT |
| Quantity = 0 | Enter quantity ≥ 1 |
| Quantity < 0 | Quantity must be positive |
| Invalid type | Only IN or OUT allowed |

## 📊 Viewing Transaction History

### Stock Index Page Shows:
- **Transaction ID**: Unique identifier (#STK-00001)
- **Product Name**: Which product was affected
- **Type Badge**: Visual IN/OUT indicator
  - 🟢 Green badge = Stock IN
  - 🔴 Red badge = Stock OUT
- **Quantity**: Amount with sign
  - Green +25 = Added 25 units
  - Red -20 = Removed 20 units
- **Date & Time**: When transaction occurred

### Typical Display:
```
| ID      | Product    | Type    | Qty   | Date                 |
|---------|------------|---------|-------|----------------------|
| #STK-00005 | Widget A | ↓ IN  | +100  | Mar 27, 2025 14:30  |
| #STK-00004 | Widget B | ↑ OUT | -50   | Mar 27, 2025 13:15  |
| #STK-00003 | Widget A | ↓ IN  | +200  | Mar 27, 2025 12:00  |
```

## 💡 Common Use Cases

### Receiving New Stock
1. Click "Add Transaction"
2. Select product: "Office Chairs"
3. Select type: "IN"
4. Enter quantity: 50
5. Submit
→ 50 office chairs added to inventory

### Processing a Sale
1. Click "Add Transaction"
2. Select product: "Desk Lamp"
3. Select type: "OUT"
4. Enter quantity: 10
5. Submit
→ 10 desk lamps removed from inventory

### Correcting Inventory
1. Click "Add Transaction"
2. Select product: "Widget C"
3. Select type: "OUT" (if over-counted)
4. Enter adjustment quantity
5. Submit
→ Inventory corrected

## 🔒 Data Integrity

### Automatic Protections:
- ✅ Cannot issue more than available stock
- ✅ All transactions timestamped
- ✅ Product stock updated atomically
- ✅ Complete audit trail maintained
- ✅ No manual stock edits needed

### What Gets Tracked:
```
StockTransaction Record:
├─ Transaction ID (auto-generated)
├─ Product ID (linked to product)
├─ Type (IN or OUT)
├─ Quantity (amount changed)
└─ Timestamp (when it occurred)

Product Update:
└─ New Quantity (recalculated)
```

## 📈 Best Practices

1. **Regular Updates**
   - Record transactions immediately
   - Don't batch multiple events

2. **Accuracy**
   - Double-check quantity before submitting
   - Use physical counts periodically

3. **Documentation**
   - Transaction timestamps provide audit trail
   - Can review history anytime

4. **Stock Monitoring**
   - Check stock levels in Products page
   - Monitor against Low Stock Threshold

## 🔗 Integration Points

### Connected Features:
- **Products Page**: Shows current stock levels
- **Low Stock Alerts**: Based on threshold
- **Reports**: Can analyze transaction history
- **Purchases**: Trigger stock IN transactions
- **Sales**: Trigger stock OUT transactions

## 🎯 Workflow Example

### Complete Inventory Scenario
```
1. Supplier delivers 100 widgets
   → Stock IN transaction for 100 units

2. Customer buys 25 widgets
   → Stock OUT transaction for 25 units

3. Quality control removes 5 defective
   → Stock OUT transaction for 5 units

4. Final inventory: 70 units
   (100 + (-25) + (-5) = 70)

5. All transactions visible in Stock page
   Date: Mar 27, 2025 | Type | Amount
```

## ❓ FAQ

**Q: Can I edit a transaction?**
A: No, transactions are immutable for audit purposes. Create a correction transaction instead.

**Q: Can I delete a transaction?**
A: No, all transactions are permanent records. Use correction transactions if needed.

**Q: What if I enter wrong quantity?**
A: If not submitted yet, edit the form. If submitted, create an opposite transaction to correct.

**Q: Does it support fractional quantities?**
A: No, quantities must be whole numbers (integer values).

**Q: Can I set the transaction date?**
A: No, timestamp is automatic (current date/time when created).

**Q: Is there a bulk transaction option?**
A: Currently single transactions only. Future enhancement may support bulk.

## 🛠️ Technical Details for Developers

### Database Table: StockTransactions
```sql
CREATE TABLE StockTransactions (
    Id int PRIMARY KEY IDENTITY(1,1),
    ProductId int FOREIGN KEY REFERENCES Products(Id),
    Type varchar(100) CHECK (Type IN ('IN', 'OUT')),
    Quantity int CHECK (Quantity > 0),
    CreatedAt datetime DEFAULT GETDATE()
);
```

### Controller Endpoint
```
GET  /Stock/Create      → Display form
POST /Stock/Create      → Process transaction
GET  /Stock/Index       → View history
```

### Validation Chain
```
HTML5 (Browser)
    ↓
Razor View Validation
    ↓
Model Annotations
    ↓
Controller ModelState
    ↓
Business Logic Check (stock availability)
    ↓
Database Save
```

## 📞 Support

For issues or questions:
1. Check validation error messages
2. Review transaction history for patterns
3. Verify product stock on Products page
4. Contact system administrator

---

**Last Updated:** March 27, 2025  
**Status:** ✅ Ready for Production  
**Build:** Successful  

