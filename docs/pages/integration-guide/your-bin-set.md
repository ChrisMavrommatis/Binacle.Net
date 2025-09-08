---
title: Your Bin Set
permalink: integration-guide/your-bin-set
nav:
  parent: Integration Guide
  order: 2
  icon: 📦
---

Choosing the right set of bins for your operation is a crucial step when integrating Binacle.Net. The bin set defines how items are packed and influences how efficiently space is utilized. Fortunately, Binacle.Net offers flexibility in this choice, allowing you to adapt it to your business needs. Below are two common approaches to bin selection and their respective advantages, disadvantages, and ideal use cases.

---

## Option 1: Using the Final Bin Destination Dimensions
In this approach, you define your bins based on the dimensions of the final lockers (e.g., shipping lockers, storage bins) where the items will be placed. The Binacle.Net API will then determine which locker can accommodate the items in a given order.

### 🔑 Advantages
- **Maximizes Space Utilization**: This option ensures that the available locker space is fully utilized.
- **Adaptability**: It allows you to easily adjust to changes in locker dimensions by simply updating your bin set.

### ⚠️ Disadvantages
- **Complex Management**: Requires tracking the bin sets for each locker provider separately, which can increase complexity.
- **Integration Complexity**: Each locker provider may need to be handled as a separate checkout option, complicating the integration process.
- **Increased Overhead**: Managing multiple locker dimensions may lead to more administrative and logistical overhead.

### 🚀 Ideal For
- Small businesses without a standardized packaging system.
- Operations that prioritize maximizing space usage and flexibility in their packing process.

---

## Option 2: Using Your Own Packaging Boxes as Bins
With this approach, you predefine a set of packaging boxes (e.g., standard shipping boxes) and assign the correct locker for each box. The Binacle.Net API will help determine which predefined box will fit all the items in an order.

### 🔑 Advantages
- **Standardized Packing**: Having a consistent set of box sizes reduces packing time and minimizes errors.
- **Simplified Inventory Management**: Managing your inventory is easier with a fixed set of box sizes.
- **Uniformity**: Standardized boxes can be used across various locker providers, providing consistency.
- **Cost Efficiency**: Bulk purchasing of boxes may reduce costs in the long run, while standardized packaging streamlines logistics.

### ⚠️ Disadvantages:
- **Limited Space Utilization**: If box sizes do not perfectly align with locker dimensions, there may be wasted space.
- **Manual Assignment**: Requires manual assignment of the correct box to each locker, which can increase the initial integration workload.

### 🚀 Ideal For:
- Larger businesses that already have or aim to implement a standardized packaging procedure.
- Companies looking to streamline operations and reduce the complexity of packing and shipping.

---

## Additional Considerations
When deciding on your bin set, keep in mind the following factors to ensure that your choice aligns with your business goals:

### 📦 Customer Experience
The speed and accuracy of order fulfillment directly impact customer satisfaction. Choose a bin set that allows your business to fulfill orders quickly and accurately, leading to better customer service.

### 📈 Scalability
As your business grows, so too will the volume of orders and complexity of logistics. Select a bin set strategy that can scale with your business needs and support growing order volumes without compromising packing efficiency.

### 💰 Cost Efficiency
Consider the long-term cost implications of your chosen bin set. Using standardized boxes may be more cost-effective through bulk purchasing and simplified logistics management.

### 🌱 Sustainability
Think about the environmental impact of your packaging. Opt for eco-friendly materials and efficient packing methods to minimize waste and align with sustainability goals.