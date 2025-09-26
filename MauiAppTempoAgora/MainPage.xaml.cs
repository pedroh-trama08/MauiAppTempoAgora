using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;
using System.Threading.Tasks;

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked_Previsao(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txt_cidade.Text))
                {
                    Tempo t = await DataService.GetPrevisao(txt_cidade.Text);

                    if (t != null) 
                    {
                        string dados_previsao = "";

                        dados_previsao = $"Latitude: {t.lat} \n" +
                                         $"Longitude: {t.lon} \n" +
                                         $"Descrição:{ t.description} \n"+
                                         $"Nascer do Sol: {t.sunrise} \n" +
                                         $"Por do Sol: {t.sunset} \n" +
                                         $"Temp. Máx: {t.temp_max} \n" +
                                         $"Temp. Mín.: {t.temp_min} \n" +
                                         $"Velocidade do Vento: {t.speed} \n" +
                                         $"Visibilidade: {t.visibility} \n";

                        lbl_res.Text = dados_previsao;

                        string mapa = $"https://embed.windy.com/embed.html?" +
                                      $"type=map&location=coordinates&metricRain=mm&metricTemp=°C" +
                                      $"&metricWind=km/h&zoom=5&overlay=wind&product=ecmwf&level=surface" +
                                      $"&lat={t.lat}&long={t.lon}";

                        wv_mapa.Source = mapa;

                    }else
                    {
                        lbl_res.Text = "Sem dados de Previsão";
                    }

                } else
                {
                    lbl_res.Text = "Preencha a cidade";
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }

        }

        private async void Button_Clicked_Localizacao(object sender, EventArgs e)
        {
            try
            {
                GeolocationRequest request =  new GeolocationRequest(
                    GeolocationAccuracy.Medium, 
                    TimeSpan.FromSeconds(10)
                    );

                Location? local = await Geolocation.Default.GetLocationAsync(request);

                if (local != null) 
                { 
                    string local_disp = $"Latitude: {local.Latitude} \n" + 
                                        $"Longitude: {local.Longitude}";

                    lbl_coords.Text = local_disp;

                    GetCidade(local.Latitude, local.Longitude); //Busca o nome da cidade que está nas coordenadas
                }

            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Erro: Dispositivo não suporta", fnsEx.Message, "OK");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                await DisplayAlert("Erro: Localização Desabilitada", fneEx.Message, "OK");
            }
            catch (PermissionException pEx)
            {
               await DisplayAlert("Erro: Permissão da Localização", pEx.Message, "OK");
            }
            catch (Exception ex)
            {
               await DisplayAlert("Erro", ex.Message, "OK");
            }


        }

        private async void GetCidade(double lat, double lon)
        {
            try
            {
                IEnumerable<Placemark> places = await Geocoding.Default.GetPlacemarksAsync(lat, lon);
            
                Placemark? place = places.FirstOrDefault();

                if (place != null) 
                {
                txt_cidade.Text = place.Locality;
                }
            } catch (Exception ex)
            {
                await DisplayAlert("Erro: Obtenção do nome da Cidade", ex.Message, "OK");
            }
               

        }
    }

}
