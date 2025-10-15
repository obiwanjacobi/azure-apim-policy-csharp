using System.Reflection;
using AzureApimPolicyGen;

var gen = new PolicyXmlGenerator(".\\policy-xml");

gen.GenerateAll(Assembly.GetExecutingAssembly());
