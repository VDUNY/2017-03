# March 2017 Presentation
## Microservices and Microsoft Azure Service Fabric
### Presented by [S. Kyle Korndoerfer](https://www.linkedin.com/in/kylekorndoerfer)
___
Micro-services are a newer approach to building large, scalable applications by breaking the program down into many smaller and independent services. When everything was in one larger/monolithic code base it was easy for one part of the code to talk to another part of the code, even if it is in a different library. But in the micro-services approach, where you deploy lots of small services with very specific functionality, how does one service talk to another service? How can you deploy multiple instances of a service and coordinate the traffic between them? How do you handle faults and ensure new services are started when needed? How do you upgrade services without downtime? How do you monitor the performance of your services?

Azure Service Fabric is a cross-platform offering from Microsoft that handles a lot of these tasks for you, leaving you to focus more on your code instead of the infrastructure. Best of all, it can run in your own data center, in Azure, or any other cloud provider... on Windows or Linux servers.