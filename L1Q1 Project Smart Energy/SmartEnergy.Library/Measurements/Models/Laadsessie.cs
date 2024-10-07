namespace SmartEnergy.Library.Measurements.Models;

public record Laadsessie(decimal prijsLaadsessie,
                          int duurLaadsessie,
                          double totaalkWhLaadsessie,
                          string startLaadsessie,
                          string stopLaadsessie);

