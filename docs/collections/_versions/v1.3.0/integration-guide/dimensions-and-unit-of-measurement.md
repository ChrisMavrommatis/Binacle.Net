---
title: Dimensions and Unit of Measurement
nav:
  parent: Integration Guide
  order: 1
  icon: 📏
---

Integrating Binacle.Net into your system requires precise handling of dimensions and units of measurement. 
Properly configuring these elements is crucial for accurate packing calculations and smooth operations. 
This guide will help you navigate the key areas for ensuring consistent and accurate dimension handling.

## 🧮 Choosing the Right Measurement Unit
By default, Binacle.Net uses **centimeters** (cm) as the unit of measurement for dimensions. 
While it’s possible to use other units, it's essential that the values are integers and adhere
to the same principles of measurement. 

For consistency and to avoid complications, we strongly recommend using centimeters throughout your integration.

- **Best Practice**: Stick to centimeters (cm) for simplicity and accuracy.

## 📏 Ensuring Consistent Dimensions
For accurate packing calculations, Binacle.Net assumes that all items are rectangular prisms (3D boxes).
Therefore, it’s essential to ensure that your product dimensions are consistent and accurate.

**Key Points:**
- **Shape**: Ensure your items are represented as rectangular prisms (length, width, height).
- **Accuracy**: Double-check all measurements to avoid errors during the packing process.

**Binacle.Net does not support irregular shapes**. If you’re dealing with non-rectangular items, 
you may need to transform them into a bounding box or use other methods before integration.

## 🔄 Conversion Considerations
If your existing measurements use units other than centimeters (e.g., millimeters or inches), you’ll need to convert
them to centimeters before passing them to Binacle.Net. 

You can perform this conversion either manually or dynamically during operation.

- **Recommended Approach**: Automate the conversion process wherever possible to avoid errors and 
maintain consistency across your system.

## ⚖️ Weight Considerations
While Binacle.Net focuses primarily on dimensions, many lockers and shipping providers impose weight limits. 
To ensure seamless integration with these systems, it's important to track the weight of products and ensure 
they stay within applicable limits.

**Considerations:**
- **Track weight in your backend system**: Make sure your system accounts for the weight of items as part of the 
overall packing logic.
- **Locker Weight Limits**: Verify the weight restrictions set by your locker or shipping provider.
- **Logic for Overweight Items**: Implement checks to prevent selecting bins or lockers that exceed weight capacity.
- **User Notifications**: Inform users of any weight-related restrictions during checkout to prevent potential 
shipping issues.

## ⚙️ Additional Best Practices
- **Measurement Accuracy**: Ensure all dimensions are measured accurately to optimize packing efficiency and avoid errors.
- **Automation**: Consider automating unit conversions and measurement handling to reduce manual work and improve consistency.
- **Standardization**: Standardize the format of dimensions across your system to ensure uniformity.
- **Use Maximum Dimensions**: When measuring products, always use the maximum dimensions (length, width, height) 
to account for slight variations and irregularities. This helps avoid issues where a package is slightly larger than expected.
