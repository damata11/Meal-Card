using Meal_Card.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Meal_Card.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        // "https://vxw7bklv-5191.uks1.devtunnels.ms/"
        // "https://vxw7bklv-63145.uks1.devtunnels.ms/"
        private readonly string _baseUrl = "https://vxw7bklv-63145.uks1.devtunnels.ms/";
        private readonly ILogger<AuthService> _logger;
        private string? _accessToken;

        JsonSerializerOptions _serializerOptions;

        public AuthService(HttpClient httpClient, ILogger<AuthService> logger)
        {

            _httpClient = httpClient;
            _logger = logger;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

        }

        public async Task<ApiResponse<bool>> Login(string? email, string? card, string senha)
        {
            try
            {
                var login = new LoginSend()
                {
                    Card = card,
                    Email = email,
                    Senha = senha
                };

                var json = JsonSerializer.Serialize(login, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // api/Utilizadores/Login
                //api/Auth/login
                var response = await PostRequest("api/Auth/login", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Erro ao enviar requisição HTTP : {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Erro ao enviar requisição HTTP : {response.StatusCode}"
                    };
                }

                var jsonResult = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Token>(jsonResult, _serializerOptions);

                if (result == null)
                {
                    return new ApiResponse<bool>
                    {
                        Data = false,
                        ErrorMessage = "Erro ao desserializar a resposta do servidor."
                    };
                }

                var accesstoken = result!.AccessToken;

                _accessToken = accesstoken;

                await SecureStorage.Default.SetAsync("accesstoken", accesstoken!.ToString());

                var utilizador = result.Utilizador;
                Preferences.Set("id", (int)utilizador!.Id);
                Preferences.Set("email", utilizador!.Email);

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro no login : {ex.Message}");
                return new ApiResponse<bool>
                {
                    ErrorMessage = $"Erro no login : {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<bool>> PostNovaSenha(string senhaCorrente, string novaSenha)
        {
            var UpdateSenha = new UpdateSenha
            {
                Corrente = senhaCorrente,
                Nova = novaSenha

            };
            try
            {
                var json = JsonSerializer.Serialize(UpdateSenha, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PutRequest("api/Utilizadores/alterar-senha", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Erro ao enviar requisição HTTP: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Erro ao enviar requisição HTTP: {response.StatusCode}"
                    };
                }

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao adicionar item ao carrinho: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> CriarPedido()
        {

            try
            {
                var json = JsonSerializer.Serialize(_serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Pedidos/criar", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Erro ao enviar requisição HTTP: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Erro ao enviar requisição HTTP: {response.StatusCode}"
                    };
                }

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao finalizar o pedido: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> GerenciarCarrinho(int id, string acao)
        {

            try
            {
                var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

                // api/PedidoItens/bar/item/5/quantidade?acao=
                var response = await PutRequest($"api/PedidoItens/bar/item/{id}/quantidade?acao={acao}", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Erro ao enviar requisição HTTP: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Erro ao enviar requisição HTTP: {response.StatusCode}"
                    };
                }

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao finalizar o pedido: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> AdicionarItemCarinho(Incluir_Carrinho carinho_Itens)
        {
            try
            {
                var json = JsonSerializer.Serialize(carinho_Itens, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/PedidoItens/bar/adicionar", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Erro ao enviar requisição HTTP: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Erro ao enviar requisição HTTP: {response.StatusCode}"
                    };
                }

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao adicionar item ao carrinho: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        private async Task<HttpResponseMessage> PostRequest(string uri, HttpContent content)
        {
            var enderecoUrl = _baseUrl + uri;
            try
            {
                var result = await _httpClient.PostAsync(enderecoUrl, content);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao enviar requisição Post para {uri} : {ex.Message}");
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }
        }

        private async Task<HttpResponseMessage> PutRequest(string uri, HttpContent content)
        {
            var enderecoUrl = _baseUrl + uri;
            try
            {
                var result = await _httpClient.PutAsync(enderecoUrl, content);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao enviar requisição Post para {uri} : {ex.Message}");
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }
        }


        public async Task<(List<Categoria>? Categorias, string? ErrorMessage)> GetCategorias(string tipo)
        {
            string endpoint = $"api/Categorias/tipo/{tipo}";
            return await GetAsync<List<Categoria>>(endpoint);
        }


        public async Task<(List<Produtos_Bar>? Produtos, string? ErrorMessage)> GetProdutosBar(string tipoProduto, string categoriaId)
        {
            string endpoint = $"api/Bar?tipoProduto={tipoProduto}&categoriaId={categoriaId}";
            return await GetAsync<List<Produtos_Bar>>(endpoint);
        }

        public async Task<(List<Produtos_Bar>? Produtos, string? ErrorMessage)> GetAllProdutosBar()
        {
            return await GetAsync<List<Produtos_Bar>>("api/Bar/produtos");
        }

        public async Task<(Produtos_Bar? Produtos, string? ErrorMessage)> GetProdutosDetalhes(int id_produto)
        {
            string endpoint = $"api/Bar/produtos/{id_produto}";
            return await GetAsync<Produtos_Bar>(endpoint);
        }

        public async Task<(List<MenuCantina>? Produtos, string? ErrorMessage)> GetRefeicoes(string tipoRefeicao, string categoriaId)
        {
            string endpoint = $"api/Cantina/produtos/tipoRefeicao/{tipoRefeicao}/categoriaId/{categoriaId}";
            return await GetAsync<List<MenuCantina>>(endpoint);
        }

        public async Task<(List<Produtos_Bar>? Produtos, string? ErrorMessage)> GetAllTransacoes()
        {
            return await GetAsync<List<Produtos_Bar>>("api/Bar/produtos");
        }

        public async Task<(Utilizador? Utilizadores, string? ErrorMessage)> GetUserInfo()
        {
            string endpoint = "api/Utilizadores/Utilizador";
            return await GetAsync<Utilizador>(endpoint);
        }

        public async Task<(Escola? Escolas, string? ErrorMessage)> GetEscolaInfo()
        {
            string endpoint = "api/Escolas/minha-escola";
            return await GetAsync<Escola>(endpoint);
        }

        public async Task<(List<Itens_Carrinho>? Carrinho, string? ErrorMessage)> GetCarrinhoAsync()
        {
            string endpoint = "api/PedidoItens/bar/carrinho";
            return await GetAsync<List<Itens_Carrinho>>(endpoint);
        }

        public async Task<(CarteiraModel? Carteira, string? ErrorMessage)> GetCarteira()
        {
            string endpoint = $"api/Carteira/minha-carteira";
            return await GetAsync<CarteiraModel>(endpoint);
        }

        public async Task<(Escola? Escolas, string? ErrorMessage)> GetSchoolInfo()
        {
            string endpoint = "api/Escolas/minha-escola";
            return await GetAsync<Escola>(endpoint);
        }

        public async Task<(List<UploadPerfil>? uploadPerfils, string? ErrorMessage)> PostImage(ImageFormat imagem)
        {
            string endpoint = $"api/Utilizadores/upload-foto/{imagem}";
            return await GetAsync<List<UploadPerfil>>(endpoint);
        }

        public async Task RecuperarTokenAsync()
        {
            if (!string.IsNullOrEmpty(_accessToken))
                return;

            _accessToken = await SecureStorage.Default.GetAsync("accesstoken");

            if (string.IsNullOrEmpty(_accessToken))
                throw new UnauthorizedAccessException("Token não encontrado");
        }

        private void AddAuthorizationHeader()
        {

            if (!string.IsNullOrEmpty(_accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            }
        }

        private async Task<(T? Data, string? ErrorMessage)> GetAsync<T>(string endpoint, bool list = false)
        {
            try
            {
                await RecuperarTokenAsync();
                AddAuthorizationHeader();

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));

                var response = await _httpClient.GetAsync(_baseUrl + endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    if (string.IsNullOrWhiteSpace(responseString))
                    {
                        return (default, "Resposta vazia do servidor");
                    }

                    var data = JsonSerializer.Deserialize<T>(responseString, _serializerOptions);
                    return (data ?? Activator.CreateInstance<T>(), null);
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        string errorMessage = "Unauthorized";
                        _logger.LogWarning(errorMessage);
                        return (default, errorMessage);
                    }

                    string generalErrorMessage = $"Erro na requisição: {response.ReasonPhrase}";
                    _logger.LogError(generalErrorMessage);
                    return (default, generalErrorMessage);
                }
            }
            catch (OperationCanceledException)
            {
                return (default, "Timeout - Verifique sua conexão com a internet");
            }
            catch (HttpRequestException ex)
            {
                string errorMessage = $"Erro de requisição HTTP: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
            catch (JsonException ex)
            {
                string errorMessage = $"Erro de desserialização JSON: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Erro inesperado: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
        }

        public async void Logout()
        {
            SecureStorage.Default.Remove("accesstoken");
            Preferences.Clear();
            await AppShell.Current.GoToAsync($"{nameof(Login)}");
        }

        public async Task<ApiResponse<bool>> UploadProfileImage(byte[] imagemArray)
        {
            try
            {
                var content = new MultipartFormDataContent();
                content.Add(new ByteArrayContent(imagemArray), "imagem", "profile.jpg");
                //Console.WriteLine($"Iniciando upload da imagem de perfil... {content}");
                var response = await PostRequest("api/Utilizadores/upload-foto", content);
                //Console.WriteLine($"Iniciando upload da imagem de perfil... {content}");

                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = response.StatusCode == System.Net.HttpStatusCode.Unauthorized ? "Unauthorized" : $"Erro ao enviar requisição HTTP: {response.StatusCode}";

                    _logger.LogError($"Erro ao enviar requisição HTTP: {response.StatusCode}");

                }
                return new ApiResponse<bool>
                {
                    Data = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao enviar imagem de perfil: {ex.Message}");
                return new ApiResponse<bool>
                {
                    ErrorMessage = $"Erro ao enviar imagem de perfil: {ex.Message}"
                };
            }
        }
    }
}