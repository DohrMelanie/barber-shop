# Price Calculation Specification

## Overview

The barber shop uses a complex pricing model that calculates the **total price per appointment** based on multiple factors. The price is calculated server-side when retrieving appointment details.

## Base Pricing by Service Type

Each `StyleReference` enum value has a different base price:

| Style Reference | Base Price |
| --------------- | ---------- |
| Short           | €25.00     |
| Medium          | €30.00     |
| Long            | €35.00     |
| Faded           | €40.00     |
| Tapered         | €38.00     |
| Undercut        | €42.00     |
| Layered         | €45.00     |
| Textured        | €48.00     |
| SlickedBack     | €35.00     |
| SideParted      | €32.00     |
| ForwardCrop     | €38.00     |
| Voluminous      | €50.00     |
| Natural         | €28.00     |
| MulletStyle     | €60.00     |
| MohawkStyle     | €65.00     |
| BeardShaped     | €15.00     |
| CleanShaven     | €12.00     |
| HotTowelShave   | €18.00     |

## Pricing Rules

### 1. Base Calculation

- Start with the sum of all service base prices for the appointment
- Price is calculated as: `Σ(Service.StyleReference.BasePrice)`

### 2. Payday Surcharge (15th of the Month)

- If the appointment date is on the **15th day of any month**, add a **25% surcharge**
- Rationale: Customers just got paid and can afford premium pricing

### 3. Sunday Premium

- If the appointment is scheduled on **Sunday**, add a **€20.00 flat surcharge**
- Rationale: Weekend work demands premium compensation

### 4. Weekday Restriction (API Validation)

- Appointments **MUST NOT** be scheduled Monday through Thursday
- Valid days: **Friday, Saturday, Sunday only**
- API should return `400 Bad Request` if appointment date falls on Monday-Thursday
- Error message: "Gerrit's Cuts is closed Monday-Thursday. We value our leisure time."

### 5. Happy Hour Discount

- If appointment `StartTime` is between **14:00 and 16:00** (2 PM - 4 PM), apply a **15% discount**
- Rationale: Attract customers during slow afternoon hours
- Discount is applied to the running total (after base + surcharges)

### 6. Beverage Surcharge

- If `BeverageChoice` is **not null or empty**, add a **€8.00 surcharge**
- Rationale: Complimentary beverage service (coffee, water, champagne for VIPs)

### 7. VIP Multiplier

- If `IsVip` is `true`, multiply the entire total by **1.5x**
- Rationale: VIP customers receive extra attention and premium service
- Applied as the final calculation step

### 8. Barber-Specific Markup

- If `BarberName` equals **"Gerrit"**, add a **20% "Master Artisan" surcharge**
- If `BarberName` equals **"Todd"**, subtract **€5.00** (apprentice discount)
- If `BarberName` is any other value, no adjustment

### 9. Duration Fee

- For every 15 minutes of `Duration` beyond the first 30 minutes, add **€2.50**
- Example: 60-minute appointment = (60-30)/15 = 2 increments = €5.00 additional

## Calculation Order

Apply rules in the following sequence:

1. Calculate base price (sum of all service base prices)
2. Add payday surcharge (+25% if applicable)
3. Add Sunday premium (+€20.00 if applicable)
4. Add barber markup (Gerrit +20%, Todd -€5.00)
5. Add duration fee (€2.50 per 15 min over 30 min)
6. Apply happy hour discount (-15% if applicable)
7. Add beverage surcharge (+€8.00 if applicable)
8. Apply VIP multiplier (×1.5 if applicable)

## Implementation Requirements

### Backend

- Add a `CalculatedPrice` property to the `Appointment` entity (computed, not stored)
- Implement price calculation logic in a service or extension method
- When retrieving appointments via API, include the calculated price in the response
- Validate appointment dates to ensure Monday-Thursday bookings are rejected

### Frontend

- Display the calculated price in the dashboard
- Show price estimate in real-time as the user builds an appointment in the editor
- Clearly indicate any surcharges/discounts applied

## Examples

### Example 1: Standard Friday Haircut

- Service: Faded (€40.00)
- Date: Friday, March 8
- Time: 10:00
- Barber: Todd
- Duration: 30 minutes
- VIP: No
- Beverage: None

**Calculation:**

1. Base: €40.00
2. Barber (Todd): €40.00 - €5.00 = €35.00
3. **Total: €35.00**

### Example 2: VIP Weekend Special

- Services: Undercut (€42.00) + BeardShaped (€15.00)
- Date: Sunday, June 15
- Time: 15:00 (Happy Hour!)
- Barber: Gerrit
- Duration: 60 minutes
- VIP: Yes
- Beverage: "Champagne"

**Calculation:**

1. Base: €42.00 + €15.00 = €57.00
2. Payday surcharge: €57.00 × 1.25 = €71.25
3. Sunday premium: €71.25 + €20.00 = €91.25
4. Barber (Gerrit): €91.25 × 1.20 = €109.50
5. Duration fee: (60-30)/15 = 2 × €2.50 = €5.00 → €109.50 + €5.00 = €114.50
6. Happy hour: €114.50 × 0.85 = €97.33
7. Beverage: €97.33 + €8.00 = €105.33
8. VIP multiplier: €105.33 × 1.5 = **€157.99**

### Example 3: Invalid Booking (Monday)

- Date: Monday, March 10
- **Result:** API returns `400 Bad Request` with error message

## Student Tasks

The student must:

1. Implement the price calculation logic in the backend
2. Add API validation to reject Monday-Thursday appointments
3. Display the calculated price in the frontend dashboard
4. (Optional) Show a real-time price estimate in the appointment editor
