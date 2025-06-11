---
title: Typical Integration Process
nav:
  parent: Integration Guide
  order: 3
  icon: 🌟
---

Integrating Binacle.Net requires some custom development, typically involving creating a module within your backend system to handle interactions with the API. Below is a detailed implementation process to guide you through integrating Binacle.Net seamlessly.



## 🔧 Step 1: Set Up Binacle.Net

**🚀 Deploy the Binacle.Net Docker Image**
- **Run Binacle.Net**: Deploy Binacle.Net using Docker on your infrastructure. You can utilize a single instance across multiple environments (testing, staging, production), as long as your presets remain consistent.

**⚙️ Configure Your Presets**
- **Customize Presets**: Define the bin presets based on your specific packaging requirements. You'll set the dimensions and capacities of the bins that you’ll use for packing. For detailed instructions, refer to the [Presets]({% vlink /configuration/core/presets.md %}) Page.


## 📦 Step 2: Integrate Locker Shipping Providers

**🛠️ Implement Locker Provider APIs**
- **API Integration**: Connect the APIs of your locker shipping provider(s). This includes making API calls for checking locker availability, placing shipment orders, and displaying locker options for users to select during checkout.

**🎨 UI/UX Design for Locker Options**
- **Clear Locker Shipping Options**: Display available locker shipping methods clearly during checkout, along with details about locker availability and any applicable conditions.

## 📏 Step 3: Manage Product Dimensions

**📝 Import Product Dimensions**
- **Consistent Data Management**: Ensure that product dimensions are consistently formatted in your backend system. Items should be represented as 3D rectangles (length, width, height) for accurate packing calculations.

**⚙️ Automate Measurement Entry**
- **Automation for Consistency**: Automate the process of entering and formatting product dimensions to reduce errors and maintain consistency across your inventory.

## 🏷️ Step 4: Call Binacle.Net at Checkout

**🌐 Make the API Call to Binacle.Net**
- **Send Product Dimensions**: When the user reaches the checkout page, make an API call to Binacle.Net with the dimensions of the items in their cart. The API will return the best-fitting locker option.

**⚖️ Check for Weight Limitations**
- **Pre-Check Weight**: Many locker providers impose weight restrictions. Before making the API call to Binacle.Net, verify that the items in the cart do not exceed the weight limits set by the provider.

**🧩 Handle API Response**
- **Response Handling**: Based on the response from Binacle.Net, either enable or disable the locker shipping option. If the items cannot fit into the available lockers, disable locker shipping and suggest alternative methods. Optionally, store the response for reference during order completion.


## 📦 Step 5: Finalize Order and Shipping

**✅ Confirm Locker Reservation**
- **Use Stored Response**: Once the user has completed their order, use the stored response from Binacle.Net to confirm which locker will be used for shipment. This ensures the correct locker is selected.

**📧 Send Confirmation with Locker Details**
- **Order Confirmation**: If the locker provider provides locker assignment details upon order completion, include these in the order confirmation email to the customer. If the locker provider handles this directly, ensure the process is smooth to offer a seamless experience.