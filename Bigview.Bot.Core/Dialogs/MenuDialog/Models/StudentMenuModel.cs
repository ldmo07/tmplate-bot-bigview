using Newtonsoft.Json;

namespace Bigview.Bot.Core.Dialogs.MenuDialog.Models;

public class StudentMenuModel
{
    [JsonProperty("periodoHistoCalificacion")]

    public long PeriodoHistoCalificacion { get; set; }

    [JsonProperty("descPeriodo")]
    public string DescPeriodo { get; set; }

    [JsonProperty("nrc")]
    public string Nrc { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("nombre")]
    public string Nombre { get; set; }

    [JsonProperty("codeNivel")]
    public string CodeNivel { get; set; }

    [JsonProperty("descNivel")]
    public string DescNivel { get; set; }

    [JsonProperty("codeAsignatura")]
    public string CodeAsignatura { get; set; }

    [JsonProperty("tituloCurso")]
    public string TituloCurso { get; set; }

    [JsonProperty("creditos")]

    public long Creditos { get; set; }

    [JsonProperty("programa")]
    public string Programa { get; set; }

    [JsonProperty("facultad")]
    public string Facultad { get; set; }

    [JsonProperty("jornada")]
    public string Jornada { get; set; }

    [JsonProperty("notaFinal")]
    public string NotaFinal { get; set; }

    [JsonProperty("descModoCalificacion")]
    public string DescModoCalificacion { get; set; }

    [JsonProperty("promedioAcumulado")]
    public string PromedioAcumulado { get; set; }

    [JsonProperty("promedioSemestre")]
    public string PromedioSemestre { get; set; }

    [JsonProperty("observacion")]
    public string Observacion { get; set; }

    [JsonProperty("resultado")]
    public string Resultado { get; set; }

    [JsonProperty("descResult")]
    public string DescResult { get; set; }
}
