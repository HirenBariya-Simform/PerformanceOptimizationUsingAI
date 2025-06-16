# POC: Performance Optimization with GitHub Copilot Agent

This is a POC to demonstrate how we can leverage an AI Agent (GitHub Copilot) to identify and resolve performance issues in a .NET Core application.


# Domain:
This is an E-commerce Application developed using ASP.NET Core API. It uses Entity Framework Core as the ORM and Microsoft SQL Server (MSSQL) as the database. The architecture follows the Repository Design Pattern.

# Key Entities
- Customer
- Order – A customer can place multiple orders.
- Product – Each order can include multiple products.
- Product Category – Each product can belong to multiple categories.



# Prompts
Step 1: Create New Branch With Basic Context And End Goal
---------------------------------------
- Create a new branch: 'feature/copilot-agent-testX' from the 'master' branch.

Step 2: Set Context for the Copilot Agent
--------------------------------------------
Set the following context for this branch:
- Goal: Identify and fix all existing performance issues in the codebase.
- Do not add new features or change existing functionality.
- Follow the current code structure and design patterns.
- If method signatures or internal logic need updates, propagate those changes consistently to dependent layers (e.g., services, controllers).
- After applying fixes, resolve any compilation errors caused by the changes.

Step 3: Identify All Performance Issues in the Application
--------------------------------------------
- In this #solution, Analyze the existing codebase for potential performance issues. Start by identifying potential slow API endpoints, Inefficient LINQ queries, unnecessary object allocations, blocking I/O operations, excessive object instantiations and Long-running operations.
- Provide a detailed list of potential performance bottlenecks with code references and clear explanation of how the issue affects performance.

Step 4: Implement Fixes for Identified Issues (One by One)
--------------------------------------------
Fix the performance issue related to Issue: 'XXXXX' and apply the fix across all relevant files in the #solution.

While applying these changes, ensure:
- No change in existing business functionality.
- No change in current design patterns.
- Method signature changes are updated throughout the codebase.
- Resolve any compilation errors caused by the changes.

Step 5. Final Review and Fix Remaining Issues
-----------------------------------------------------
Perform a final re-analysis of the #solution to detect any remaining performance bottlenecks.
- Identify and provide remaining issues (If Any) with the code references and reasoning.
- Do not introduce any new features at this stage.
- Confirm all previously identified issues are fixed.
