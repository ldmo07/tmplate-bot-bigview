using Bigview.Bot.Core.Dialogs.MenuDialog.Models;
using Bigview.Bot.Core.Properties;

using Newtonsoft.Json;

namespace Bigview.Bot.Core.Dialogs.MenuDialog.Services;

public class StudentServiceAdapter
{
    private readonly HttpClient http;

    public StudentServiceAdapter(IHttpClientFactory httpClientFactory)
    {
        this.http = httpClientFactory.CreateClient("student");
    }

    public async Task<StudentMenuModel> GetStudentAsync(string studentId)
    {
        var response = await http.GetAsync($"estudiantes/Estudiantes/CalificacionesHistorico?uid_usuario={studentId}&codeSede=ZIP&codenivel=TC&codePrograma=TINF");

        response.EnsureSuccessStatusCode();

        string studentModelResponse = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(studentModelResponse))
            throw new ArgumentException(Resources.StudentServiceAdapterNotFound);

        if (studentModelResponse.Contains("El uid_usuario no existe"))
            throw new ArgumentException(Resources.StudentServiceAdapterNotFound);

        return JsonConvert.DeserializeObject<StudentMenuModel[]>(studentModelResponse).FirstOrDefault();
    }

}
