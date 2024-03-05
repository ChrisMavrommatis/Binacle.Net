# Binacle.NET

Binacle.NET is a .NET library for attempting to solve the bin fitting problem in one dimension only.

It is designed to be integrated into websites that want to ship orders into self-served locker systems until the customer retrieves them.

It utilizes a heuristic algorithm to provide a fast way to determine if all of the items will fit into a bin (container), so the site can choose to display the option to ship to a locker system.

Additionally, given a set of bins (container) sizes it will also determine which one it fits into, so the site can choose the best container for the order.

The library utilizes some sample container sizes but it can be customized to be used to fit items into a set of predefined packaging box sizes.



# Algorithm

This library utilizes a hybrid variant of the First Fit Decreasing algorithm in order to solve the bin packing problem in 1 dimension.

Many heuristics have been developed to solve the bin packing problem, and the First Fit Decreasing algorithm is one of the most popular. 



