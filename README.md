
## POC: Performance Optimization with GitHub Copilot Agent
This repository demonstrates a Proof of Concept (POC) to demonstrate how we can use an AI Agent (GitHub Copilot) to identify and resolve performance issues in a .NET Core application. It provide step by step guide on how to use AI prompts to Identify, review and fix the performance issues in dotnet core application. It intended to mimise the human efforts in identifying and resolving issue in code base.

## Domain:
This is an E-commerce application developed using ASP.NET Core API. It uses entity framework Core as the ORM with microsoft SQL server (MSSQL) database and the application architecture follows the repository design pattern.


## Key Database Entities
- Customer 
- Order: A customer can place multiple orders.
- Product: Each order can include multiple products.
- Product Category: Each product can belong to multiple categories.


## Step-by-Step Guide

### Step 1: Create New Branch With Basic Context And End Goal
Prompt:
- Create a new branch: 'feature/copilot-agent-testX' from the 'master' branch.

### Step 2: Identify the performance issues in our application
Prompt:
- In this #solution, Analyze the code for performance Issues. For eg: Heavy LINQ queries, blocking I/O operations, excessive object instantiations, unnecessary allocations, slow API endpoints and other issues that impact the performance of my application.
- So Review code and provide a list of potential performance bottlenecks with code references and detailed explaination about what is the issue and how it can impact the performace.
- Provide the issues as per the priority from high to low severity and its impact. 
- We are not considering to introduce any new feature until all the existing performance issues gets resolved so do not suggest new features.

### Step 3: Implement Fix for identified issues 
#### Approach 1: Implement Fixes for All Identified Issues (One by One)
##### Step 3.1.1: Understand perticular issue in more details:
Prompt:
- Regarding the issue: 'XXXXXXXXX' in this #solution, Provide specific code references and explain in more details about - how is the current implementation around that issue, how that issue can impact the performance and what could be the optimal solution around that?

##### Step 3.1.2: Fix the perticular issue:
Prompt:
- Please review codebase of the #solution and implement appropriate fixes for all identified Issues around: 'XXXXXXXXX'.
While applying these changes, ensure that:
- The existing application functionality remains unaffected. and all fixes should follow the existing design pattern.
- Any updates to method names or signatures (e.g., at the repository level) are consistently propagated to higher-level components such as service classes or controllers.
- Any resulting compilation issues due to these changes are identified and resolved appropriately.

#### Approach 2:  Implement Fixes for All Identified Issues
##### Step 3.2.1: Implement Fixes for All Identified Issues (One by One)

- For all these identified performance issues, Implement the fixes for all these issues in all related code files in the #solution.
While applying these changes, ensure that:
- The existing application functionality remains unaffected and all fixes should follow the current code structure and design patterns.
- Any updates to method names or signatures (e.g., at the repository level) are consistently propagated to higher-level components such as service classes or controllers.
- Any resulting compilation issues due to these changes are identified and resolved appropriately.
- The goal is to fix this performance issue without introducing regressions or breaking existing behavior.

### Step 4. Final Review and Fix Remaining Issues
Prompt:
- Please re-analyze the existing codebase within the #solution to identify any remaining performance issues.
- Provide a comprehensive list of potential performance bottlenecks, including specific code references and detailed explanations of how each issue affects application performance (e.g., memory usage, response time, resource contention).
- Note: At this stage, we are strictly focused on resolving all current performance issues. No new functionality should be introduced until these concerns are fully addressed.

## License
This POC is provided for demonstration and educational purposes. Adapt and extend for your own use.
