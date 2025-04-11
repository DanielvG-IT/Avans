using Dapper;
using HQB.WebApi.Models;
using HQB.WebApi.Interfaces;
using Microsoft.Data.SqlClient;

namespace HQB.WebApi.Repositories
{
    public class PersonalAppointmentsRepository : IPersonalAppointmentsRepository
    {
        private readonly string _connectionString;

        public PersonalAppointmentsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<PersonalAppointments>> GetPersonalAppointments()
        {
            using var connection = new SqlConnection(_connectionString);
            const string sqlQuery = "SELECT * FROM PersonalAppointments";
            return await connection.QueryAsync<PersonalAppointments>(sqlQuery);
        }

        public async Task<IEnumerable<PersonalAppointments>> GetPersonalAppointmentsByPatientId(Guid patientId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sqlQuery = "SELECT * FROM PersonalAppointments WHERE PatientID = @PatientId";
            return await connection.QueryAsync<PersonalAppointments>(sqlQuery, new { PatientId = patientId });
        }

        public async Task<PersonalAppointments?> GetPersonalAppointmentById(Guid id)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sqlQuery = "SELECT * FROM PersonalAppointments WHERE ID = @Id";
            return await connection.QueryFirstOrDefaultAsync<PersonalAppointments>(sqlQuery, new { Id = id });
        }

        public async Task<int> AddPersonalAppointment(PersonalAppointments personalAppointment)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sqlQuery = @"
                INSERT INTO PersonalAppointments 
                    (ID, PatientID, AppointmentID, AppointmentDate, CompletedDate, CompletedQuestion, Sequence) 
                VALUES 
                    (@Id, @PatientId, @AppointmentId, @AppointmentDate, @CompletedDate, @CompletedQuestion, @Sequence)";
            return await connection.ExecuteAsync(sqlQuery, personalAppointment);
        }

        public async Task<int> UpdatePersonalAppointment(PersonalAppointments personalAppointment)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sqlQuery = @"
                UPDATE PersonalAppointments 
                SET 
                    PatientID = @PatientId, 
                    AppointmentID = @AppointmentId, 
                    AppointmentDate = @AppointmentDate, 
                    CompletedDate = @CompletedDate, 
                    CompletedQuestion = @CompletedQuestion, 
                    Sequence = @Sequence 
                WHERE ID = @Id";
            return await connection.ExecuteAsync(sqlQuery, personalAppointment);
        }

        public async Task<int> DeletePersonalAppointment(Guid id)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sqlQuery = "DELETE FROM PersonalAppointments WHERE ID = @Id";
            return await connection.ExecuteAsync(sqlQuery, new { Id = id });
        }

        public async Task<int> MarkAppointmentAsCompleted(Guid id)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sqlQuery = "UPDATE PersonalAppointments SET CompletedDate = @CompletedDate WHERE ID = @Id";
            return await connection.ExecuteAsync(sqlQuery, new { Id = id, CompletedDate = DateTime.UtcNow });
        }

        public async Task<int> MarkQuestionAsCompleted(Guid id)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sqlQuery = "UPDATE PersonalAppointments SET completedQuestion = @CompletedQuestion WHERE ID = @Id";
            return await connection.ExecuteAsync(sqlQuery, new { Id = id, CompletedQuestion = true });
        }
    }
}