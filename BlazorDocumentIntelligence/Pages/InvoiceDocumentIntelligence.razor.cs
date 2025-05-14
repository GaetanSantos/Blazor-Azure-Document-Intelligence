using BlazorDocumentIntelligence.Models;
using BlazorDocumentIntelligence.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorDocumentIntelligence.Pages
{
    public class InvoiceDocumentIntelligenceModel : ComponentBase
    {
        [Inject] 
        protected IAzureDocumentIntelligenceService DocumentIntelligenceService { get; set; } = default!;
        [Inject] 
        protected IAzureBlobStorageService BlobStorageService { get; set; } = default!;

        protected InvoiceAnalysis? _invoiceAnalysis;
        protected bool _isAnalyzing;
        protected bool _canAnalyze = false;
        protected string? _uploadedFileUrl;
        protected string? _snackbarMessage;
        protected string _snackbarCssClass = "bg-success";
        protected bool _snackbarVisible;

        protected async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
            _canAnalyze = false;
            var file = e.File;
            if (file != null)
            {
                using var stream = file.OpenReadStream();
                var fileName = $"{Guid.NewGuid()}_{file.Name}";
                try
                {
                    _uploadedFileUrl = await BlobStorageService.UploadAsync(stream, fileName);
                    _canAnalyze = true;
                    ShowSnackbar("File uploaded to Azure Blob Storage successfully.");
                }
                catch
                {
                    _canAnalyze = false;
                    ShowSnackbar("Error uploading file to Azure Blob Storage.", isError: true);
                }
            }
        }

        protected async Task AnalyzeDocumentAsync()
        {
            if (string.IsNullOrEmpty(_uploadedFileUrl))
                return;

            try
            {
                _isAnalyzing = true;
                _invoiceAnalysis = await DocumentIntelligenceService.AnalyzeAsync(_uploadedFileUrl);
                ShowSnackbar("Document analysis succeeded.");
            }
            catch
            {
                ShowSnackbar("Error during document analysis.", isError: true);
            }
            finally
            {
                _isAnalyzing = false;
            }
        }

        protected void ShowSnackbar(string message, bool isError = false)
        {
            _snackbarMessage = message;
            _snackbarCssClass = isError ? "bg-danger" : "bg-success";
            _snackbarVisible = true;
            StateHasChanged();
            _ = HideSnackbarAfterDelay();
        }

        protected async Task HideSnackbarAfterDelay()
        {
            await Task.Delay(5000);
            _snackbarVisible = false;
            StateHasChanged();
        }

        protected Task HideSnackbar()
        {
            _snackbarVisible = false;
            StateHasChanged();
            return Task.CompletedTask;
        }
    }
}
