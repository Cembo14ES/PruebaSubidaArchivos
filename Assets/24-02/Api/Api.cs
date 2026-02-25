using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Program : MonoBehaviour
{
    private static readonly HttpClient Http = new HttpClient();

    [Header("UI")]
    public TMP_Text respuesta;
    public Button button;
    public TMP_InputField inputField;

    // 🔹 Historial persistente (NO se reinicia)
    private List<Dictionary<string, object>> history = new List<Dictionary<string, object>>();

    // 🔹 Instrucciones del sistema
    private string instructions =
        "Eres un monje del Monasterio de Irache, pero hablas de forma cercana y coloquial. " +
        "Usas expresiones sencillas, naturales y actuales. " +
        "Mantienes un tono amable y tranquilo, pero sin lenguaje antiguo ni demasiado formal. " +
        "Si te preguntan en euskera, contestas en euskera.";

    string apiKey = "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"; // <-- PON TU API AQUÍ

    private void Start()
    {
        button.onClick.AddListener(EnviarMensaje);
        inputField.characterLimit = 120;

        Http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);
    }

    public async void EnviarMensaje()
    {
        string userText = inputField.text;

        if (string.IsNullOrWhiteSpace(userText))
            return;

        button.interactable = false; // evitar doble click

        // 🔹 Añadimos mensaje del usuario al historial
        history.Add(new Dictionary<string, object>
        {
            ["role"] = "user",
            ["content"] = userText
        });

        var payload = new
        {
            model = "gpt-5-mini",
            instructions = instructions,
            input = history
        };

        string json = JsonConvert.SerializeObject(payload);

        try
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.openai.com/v1/responses");

            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await Http.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Debug.LogError(responseBody);
                respuesta.text = "Error en la API.";
                button.interactable = true;
                return;
            }

            var jsonResponse = JObject.Parse(responseBody);
            var output = jsonResponse["output"];

            string assistantText = "";

            foreach (var item in output)
            {
                if (item["type"]?.ToString() == "message")
                {
                    foreach (var content in item["content"])
                    {
                        if (content["type"]?.ToString() == "output_text")
                        {
                            assistantText += content["text"]?.ToString();
                        }
                    }
                }
            }

            // 🔹 Guardamos respuesta en historial
            history.Add(new Dictionary<string, object>
            {
                ["role"] = "assistant",
                ["content"] = assistantText
            });

            respuesta.text = "Monje: " + assistantText;
            inputField.text = "";
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            respuesta.text = "Error de conexión.";
        }

        button.interactable = true;
    }
}
