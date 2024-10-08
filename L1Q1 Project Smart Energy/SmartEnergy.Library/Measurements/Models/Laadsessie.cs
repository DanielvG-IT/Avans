namespace SmartEnergy.Library.Measurements.Models;
public record Laadsessie(DateTime Start,
                          DateTime Stop,
                          TimeSpan Duur,
                          decimal Prijs,
                          double TotaalKWh
                          );