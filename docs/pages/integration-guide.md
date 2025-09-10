---
title: Integration Guide
permalink: integration-guide
nav:
  order: 3
  icon: 🔗
---

Integrate Binacle.Net by preparing product data, configuring bins, connecting your backend to the API, 
and optimizing for performance.

---

## 📌 Integration Essentials

- Structure product dimensions as integer values (centimeters are highly reccomended)
- Define bin sets to match your packing environment
- Connect to Binacle.Net’s API for live packing automation
- Ensure requests are fast and efficient

---

## 📏 Dimensions & Units

- Use **centimeters** for all measurements, keep values as integers
- Represent products as rectangular prisms (length, width, height), irregular shapes must be boxed
- Convert other units to centimeters—automate this for consistency
- Track product **weight** and enforce provider limits during packing
- Automate entries, standardize dimension format, and always use maximum measurements to prevent errors

---

## 📦 Selecting Your Bin Set

**Approach 1: Final Destination Dimensions**
- Define bins by locker/storage sizes
- Maximizes space, and eases provider changes
- Requires tracking bin specs per provider and increases complexity

**Approach 2: Standardized Packaging Boxes**
- Predefine box sizes as bins
- Simple inventory, consistent packing, potential for bulk cost savings
- May waste space if boxes don't match lockers well, initial setup will require manual labor

**Consider:**
- Impact on customer experience
- Scalability to higher order volumes
- Long-term logistics and packaging costs
- Environmental goals, choose efficient, sustainable packing materials

---

## 🌟 Integration Process Steps

**Step 1: Deploy Binacle.Net**
- Run via Docker. Share instances if using consistent configuration
- Set bin presets. Refer to the Presets documentation for you Binacle.Net version.

**Step 2: Integrate Locker Shipping Providers**
- Connect provider APIs to check availability, create shipments, and display options at checkout
- Ensure UI is clear and informative

**Step 3: Manage Product Dimensions**
- Import and format product length, width, height in backend
- Automate measurement entry to minimize errors

**Step 4: Checkout API Call**
- On checkout, send item dimensions to Binacle.Net
- Verify cart weight against locker limits
- Enable locker shipping if items fit, otherwise offer alternatives.
- Store API results.

**Step 5: Finalize Shipping**
- Confirm locker reservation using saved API data
- Include locker assignment in the confirmation email, if provided.
