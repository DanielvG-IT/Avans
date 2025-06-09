using System;
using System.ComponentModel.DataAnnotations;

namespace TralaAI.CoreApi.Models
{

    public class AggregatedTrashDto
    {
        public Guid Id { get; set; }
        public string? Type { get; set; }
        public DateTime Date { get; set; }
        public double Confidence { get; set; }
        public string? Weather { get; set; }
        public int Temperature { get; set; }
    }

}