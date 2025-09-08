---
title: Typical Integration Process
permalink: integration-guide/typical-integration-process
nav:
  parent: Integration Guide
  order: 3
  icon: 🌟
---

Integrating **Binacle.Net** involves custom backend development, typically via a dedicated module for API interactions. Follow these steps for a smooth integration.

---

## 🔧 Step 1: Set Up Binacle.Net

- **🚀 Deploy the Docker Image:**  
  Run Binacle.Net using Docker on your infrastructure. You may share an instance across test, staging, and production environments if presets are consistent.
- **⚙️ Configure Presets:**  
  Define your bin presets (dimensions and capacity) matched to your packaging needs. Refer to the Presets page in your version’s documentation for detailed instructions.

---

## 📦 Step 2: Integrate Locker Shipping Providers

- **🛠️ Connect Locker Provider APIs:**  
  Integrate APIs from locker shipping providers. Common tasks include:
    - Checking locker availability
    - Placing shipment orders
    - Displaying locker choices at checkout
- **🎨 Design UI/UX for Locker Shipping:**  
  Make locker shipping options visible and clear to users during checkout, showing locker details and any restrictions.

---

## 📏 Step 3: Manage Product Dimensions

- **📝 Import Product Measurements:**  
  Ensure product dimensions—length, width, and height—are formatted consistently in your backend for accurate packing calculations.
- **⚙️ Automate Measurement Entry:**  
  Implement automated systems to enter and format dimensions, helping maintain data consistency and minimizing errors.

---

## 🏷️ Step 4: Checkout: Call Binacle.Net

- **🌐 API Call at Checkout:**  
  When users reach checkout, send item dimensions to Binacle.Net’s API to get the best locker fit suggestion.
- **⚖️ Pre-Check Weight Restrictions:**  
  Before sending the API call, confirm cart items stay within provider weight limits.
- **🧩 Handle the API Response:**  
  Enable or disable locker shipping based on Binacle.Net's response. If no locker fits, suggest alternatives. Optionally, store the API response for order completion.

---

## 📦 Step 5: Finalize Order and Shipping

- **✅ Confirm Locker Reservation:**  
  Use the stored Binacle.Net response to confirm and assign the correct locker for shipping after order completion.
- **📧 Send Locker Details in Confirmation:**  
  Include locker assignment details in the order confirmation email if provided by the locker service. For provider-managed notifications, ensure user experience remains smooth.
