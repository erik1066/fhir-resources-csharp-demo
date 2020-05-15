using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Hl7.Fhir.Model; // Required for being able to use the C# class representations of FHIR resources
using Hl7.Fhir.Serialization; // Required for converting FHIR C# objects into and out of Json using the FhirJsonSerializer class

// This project requires the Hl7.Fhir.R4 package from Nuget.org

namespace fhir_resources_csharp_demo.Controllers
{
    /* Important note: This controller doesn't implement the FHIR API specification. I'm just
     * showing how to create and modify FHIR resources in C# using the FHIR SDK for .NET.
     */
    [ApiController]
    [Route("[controller]")]
    public class FhirController : ControllerBase
    {
        private readonly ILogger<FhirController> _logger;

        private static FhirJsonSerializer _fhirJsonSerializer = new FhirJsonSerializer(new SerializerSettings()
        {
            Pretty = true
        });

        public FhirController(ILogger<FhirController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Produces("application/fhir+json")]
        public IActionResult GetValueSetById([FromBody] Dictionary<string, string> patientProperties)
        {
            // Create the FHIR Patient resource using the standard C# syntax
            Patient patient = new Patient();

            // A patient can have more than one name, which is why patient.Name is of type List<HumanName> instead of just HumanName
            patient.Name = new List<HumanName>()  
            { 
                // We will assume for this demo that the Patient object we're creating will only
                // have one name. Therefore, we will initialize the patient's list of names with a 
                // default HumanName already in the list. We will then just this default item
                // every time we want to modify the Patient's name.
                new HumanName() 
            };

            // Like Name, a patient can also have multiple addresses, hence Address is a list
            patient.Address = new List<Address>()
            { 
                // We will assume for this demo that the Patient object we're creating will only
                // have one address.
                new Address()
            };

            foreach (var keyValuePair in patientProperties)
            {
                if (keyValuePair.Key == "identifier")
                {
                    patient.Identifier = new List<Identifier>()
                    {
                        new Identifier()
                        {
                            Value = keyValuePair.Value,
                            Use = Identifier.IdentifierUse.Official
                        }
                    };
                }
                if (keyValuePair.Key == "active")
                {
                    bool.TryParse(keyValuePair.Value, out bool active);
                    patient.Active = active;
                }
                if (keyValuePair.Key == "name.family")
                {
                    patient.Name.FirstOrDefault().Family = keyValuePair.Value;
                }
                else if (keyValuePair.Key == "name.given")
                {
                    // Patients can have more than one given name which is why this is a List of string instead of just a string
                    patient.Name.FirstOrDefault().Given = new List<string>() { keyValuePair.Value };
                }

                if (keyValuePair.Key == "address.line")
                {
                    // An address can have multiple lines for the street. Ex: "1234 Main St Apt 100" would be two lines.
                    // Thus, the 'Line' property of 'Address' is represented as a list instead of just a string.
                    // But we're going to treat this as just a single line for demonstration purposes.
                    patient.Address.FirstOrDefault().Line = new List<string>()
                    {
                        keyValuePair.Value
                    };
                }

                if (keyValuePair.Key == "address.city")
                {
                    patient.Address.FirstOrDefault().City = keyValuePair.Value;
                }
                if (keyValuePair.Key == "address.district")
                {
                    patient.Address.FirstOrDefault().District = keyValuePair.Value;
                }
                if (keyValuePair.Key == "address.state")
                {
                    patient.Address.FirstOrDefault().State = keyValuePair.Value;
                }
                if (keyValuePair.Key == "address.postalCode")
                {
                    patient.Address.FirstOrDefault().PostalCode = keyValuePair.Value;
                }
                if (keyValuePair.Key == "address.country")
                {
                    patient.Address.FirstOrDefault().Country = keyValuePair.Value;
                }

                if (keyValuePair.Key == "maritalStatus")
                {
                    // The concept codes for marital status are specified in the Patient documentation at https://www.hl7.org/fhir/patient.html.
                    // But for ease of reading this code, you can just go to https://www.hl7.org/fhir/valueset-marital-status.html to find the codes I'm using.
                    var coding = new Coding();

                    // Note that in an ideal development scenario, where all you are provided is the concept code,
                    // you would call a FHIR vocabulary API to get the display value for that code and possibly
                    // the code system information. But that's beyond the scope of this demo, so we're just going
                    // to hard-code it. This does mean we need a lot of if-then statements, though.
                    if (keyValuePair.Value.Equals("A"))
                    {
                        coding.Code = keyValuePair.Value;
                        coding.System = "http://terminology.hl7.org/CodeSystem/v3-MaritalStatus";
                        coding.Display = "Annulled";
                    }
                    else if (keyValuePair.Value.Equals("D"))
                    {
                        coding.Code = keyValuePair.Value;
                        coding.System = "http://terminology.hl7.org/CodeSystem/v3-MaritalStatus";
                        coding.Display = "Divorced";
                    }
                    else if (keyValuePair.Value.Equals("I"))
                    {
                        coding.Code = keyValuePair.Value;
                        coding.System = "http://terminology.hl7.org/CodeSystem/v3-MaritalStatus";
                        coding.Display = "Interlocutory";
                    }
                    else if (keyValuePair.Value.Equals("L"))
                    {
                        coding.Code = keyValuePair.Value;
                        coding.System = "http://terminology.hl7.org/CodeSystem/v3-MaritalStatus";
                        coding.Display = "Legally Separated";
                    }
                    else if (keyValuePair.Value.Equals("M"))
                    {
                        coding.Code = keyValuePair.Value;
                        coding.System = "http://terminology.hl7.org/CodeSystem/v3-MaritalStatus";
                        coding.Display = "Married";
                    }
                    else if (keyValuePair.Value.Equals("P"))
                    {
                        coding.Code = keyValuePair.Value;
                        coding.System = "http://terminology.hl7.org/CodeSystem/v3-MaritalStatus";
                        coding.Display = "Polygamous";
                    }
                    else if (keyValuePair.Value.Equals("S"))
                    {
                        coding.Code = keyValuePair.Value;
                        coding.System = "http://terminology.hl7.org/CodeSystem/v3-MaritalStatus";
                        coding.Display = "Never Married";
                    }
                    else if (keyValuePair.Value.Equals("T"))
                    {
                        coding.Code = keyValuePair.Value;
                        coding.System = "http://terminology.hl7.org/CodeSystem/v3-MaritalStatus";
                        coding.Display = "Domestic partner";
                    }
                    else if (keyValuePair.Value.Equals("U"))
                    {
                        coding.Code = keyValuePair.Value;
                        coding.System = "http://terminology.hl7.org/CodeSystem/v3-MaritalStatus";
                        coding.Display = "unmarried";
                    }
                    else if (keyValuePair.Value.Equals("W"))
                    {
                        coding.Code = keyValuePair.Value;
                        coding.System = "http://terminology.hl7.org/CodeSystem/v3-MaritalStatus";
                        coding.Display = "Widowed";
                    }
                    else if (keyValuePair.Value.Equals("UNK"))
                    {
                        coding.Code = keyValuePair.Value;
                        coding.System = "http://terminology.hl7.org/CodeSystem/v3-NullFlavor";
                        coding.Display = "unknown";
                    }

                    if (!string.IsNullOrEmpty(coding.Code))
                    {
                        patient.MaritalStatus = new CodeableConcept()
                        {
                            Coding = new List<Coding>()
                            {
                                coding
                            }
                        };
                    }
                }

                if (keyValuePair.Key == "race")
                {
                    // Race isn't defined in the FHIR Patient resource. It's an extension based on US-Core. 
                    // So, this code shows how we can create extensions (like Race) using the C# FHIR SDK.
                    var race = new Extension
                    {
                        Url = "http://hl7.org/fhir/us/core/StructureDefinition/us-core-race"
                    };

                    var raceRoding = new Coding();

                    if (keyValuePair.Value.Equals("American Indian or Alaska Native"))
                    {
                        raceRoding.Code = "1002-5";
                        raceRoding.System = "urn:oid:2.16.840.1.113883.6.238";
                        raceRoding.Display = keyValuePair.Value;
                    }
                    else if (keyValuePair.Value.Equals("Asian"))
                    {
                        raceRoding.Code = "2028-9";
                        raceRoding.System = "urn:oid:2.16.840.1.113883.6.238";
                        raceRoding.Display = keyValuePair.Value;
                    }
                    else if (keyValuePair.Value.Equals("Black or African American"))
                    {
                        raceRoding.Code = "2054-5";
                        raceRoding.System = "urn:oid:2.16.840.1.113883.6.238";
                        raceRoding.Display = keyValuePair.Value;
                    }
                    else if (keyValuePair.Value.Equals("Native Hawaiian or Other Pacific Islander"))
                    {
                        raceRoding.Code = "2076-8";
                        raceRoding.System = "urn:oid:2.16.840.1.113883.6.238";
                        raceRoding.Display = keyValuePair.Value;
                    }
                    else if (keyValuePair.Value.Equals("White"))
                    {
                        raceRoding.Code = "2106-3";
                        raceRoding.System = "urn:oid:2.16.840.1.113883.6.238";
                        raceRoding.Display = keyValuePair.Value;
                    }

                    // A real patient could have multiple races and the US-Core extension supports that. For
                    // demo purposes, we'll assume our input data from the Web API is just supplying one race.
                    // This will keep the code simple and it will be easier to understand.
                    var ombCategory = new Extension()
                    {
                        Url = "ombCategory",
                        Value = raceRoding
                    };
                    race.Extension.Add(ombCategory);

                    if (!string.IsNullOrEmpty(raceRoding.Code))
                    {
                        // This is all there is to adding 'Race' as an extension
                        patient.Extension.Add(race);
                    }
                }
            }

            patient.Name.FirstOrDefault().Text = $"{patient.Name.FirstOrDefault().Given.FirstOrDefault()} {patient.Name.FirstOrDefault().Family}";

            string fhirContent = _fhirJsonSerializer.SerializeToString(patient);
            return Content(fhirContent);
        }
    }
}
