Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Runtime.Serialization
Imports Newtonsoft.Json
Imports NewsAPI
Imports NewsAPI.Models
Imports NewsAPI.Constants

Public Class Funcoes


    'Const APICode As String = ""
    'Const COMMA As String = ","
    'Dim ZipCode As String
    'Dim city, temp, rain, wind, data, fileNameString As String
    'Private Function GetWBInfo(ByVal ZipCode As String) As WeatherInfo
    '    Try
    '        Dim URL As String = "http://" + APICode + "api.wxbug.net/getLocationsXML.aspx?ACode=" + APICode + "&SearchString="
    '        Dim DS As DataSet = New DataSet
    '        DS.ReadXml(URL)
    '        Dim RV As New WeatherInfo(DS, ZipCode)
    '        Return RV
    '    Catch ex As Exception
    '        Return Nothing
    '    End Try
    'End Function



    'Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

    '    Dim URL As String = "http://" + APICode + ".api.wxbug.net/getLiveWeatherRSS.aspx?ACode=" + APICode + "&zipCode=" + ZipCode
    '    Dim request As HttpWebRequest
    '    Dim response As HttpWebResponse = Nothing
    '    Dim dsWeather As DataSet
    '    Dim fileReader As New StreamReader(fileNameString)

    '    Do While fileReader.Peek <> -1

    '        ZipCode = fileReader.ReadLine
    '        URL = "http://" + APICode + ".api.wxbug.net/getLiveWeatherRSS.aspx?ACode=" + APICode + "&zipCode=" + ZipCode

    '        Try
    '            request = DirectCast(WebRequest.Create(URL), HttpWebRequest)

    '            response = DirectCast(request.GetResponse(), HttpWebResponse)

    '            dsWeather = New DataSet

    '            dsWeather.ReadXml(response.GetResponseStream())

    '            'PrintDataSet(dsWeather)
    '            city = dsWeather.Tables("city-state").Rows(0).Item("city-state_Text")
    '            temp = dsWeather.Tables("temp").Rows(0).Item("temp_Text")
    '            rain = dsWeather.Tables("rain-today").Rows(0).Item("rain-today_Text")
    '            wind = dsWeather.Tables("wind-speed").Rows(0).Item("wind-speed_Text")
    '            data = city & COMMA & temp & COMMA & rain & COMMA & wind
    '            TextBox1.Text = data + Environment.NewLine

    '        Catch ex As Exception

    '        Finally
    '            If Not response Is Nothing Then response.Close()
    '        End Try
    '    Loop


End Class

'' CLasse Noticia
Public Class Noticia

    Shared appId As String = ""

    Public Shared Function carregarNoticias(ByVal assunto As String)
        ''https://newsapi.org/v2/top-headlines?country=br&apiKey=
        'Dim url = "https://newsapi.org/v2/top-headlines?country=br&apiKey=" & appId
        'Dim url = "https://newsapi.org/v2/top-headlines?country=br&sources=google-news&apiKey=" & appId
        Dim url = "https://newsapi.org/v2/top-headlines?country=br&category=technology&apiKey=" & appId

        If Not String.IsNullOrEmpty(assunto) Then
            url = "https://newsapi.org/v2/top-headlines?country=br&q=" & assunto & "&apiKey=" & appId
        End If

        Dim noticias As New Noticias

        Try
            Dim client As WebClient = New WebClient()
            client.Encoding = System.Text.Encoding.UTF8

            Dim json = client.DownloadString(url)

            noticias = JsonConvert.DeserializeObject(Of Noticias)(json)

        Catch ex As Exception

        End Try

        Return noticias
    End Function

    Public Class Noticias
        Public Property status As String
        Public Property totalResults As Integer
        Public Property articles As List(Of Article)
    End Class

    Public Class Source
        Public Property id As String
        Public Property name As String
    End Class

    Public Class Article
        Public Property source As Source
        Public Property author As String
        Public Property title As String
        Public Property description As String
        Public Property url As String
        Public Property urlToImage As String
        Public Property publishedAt As DateTime
    End Class

End Class

'' PEGA A PREVISAO DO TEMPO
Public Class PrevisaoTempo
    Shared appIdMinha As String = ""
    Shared appIdMinha2 As String = ""
    Shared appIdWeb As String = ""

    Shared baseUrl As String = "http://api.openweathermap.org/data/2.5/find/city?lat={0}&lon={1}&cnt={2}"

    'Public Shared Function PrevisaoTempoCidade(ByVal nome As String) As WeatherInfo
    Public Shared Function PrevisaoTempoCidade(ByVal nome As String) As Previsao
        Dim client = New WebClient()
        client.Encoding = Encoding.UTF8
        client.Headers.Add("User-Agent", "Nobody")

        Dim url As String =
            String.Format("http://api.openweathermap.org/data/2.5/forecast/daily?q={0}&units=metric&cnt=1&APPID={1}&lang=pt", nome.Trim(), appIdWeb)
        Try
            Dim response = client.DownloadString(New Uri(url))
            'Dim previsao As WeatherInfo = JsonConvert.DeserializeObject(Of WeatherInfo)(response)
            Dim previsao As Previsao = JsonConvert.DeserializeObject(Of Previsao)(response)
            Return previsao
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    'Public Shared Function PrevisaoTempoCoordenadas(latitude As Double, longitude As Double, quantidade As Integer) As WeatherInfo
    '    Dim url = String.Format(baseUrl, latitude, longitude, quantidade) & "APPID=" & appId
    '    Try
    '        ' Chamada sincrona
    '        Dim syncClient = New WebClient()
    '        Dim content = syncClient.DownloadString(url)
    '        ' Cria o serializados Json e trata a resposta
    '        Dim serializer As New DataContractJsonSerializer(GetType(WeatherInfo))

    '        Using ms = New MemoryStream(Encoding.Unicode.GetBytes(content))
    '            ' deserializa o objeto JSON usando o tipo de dados
    '            Dim weatherData = DirectCast(serializer.ReadObject(ms), WeatherInfo)
    '            Return weatherData
    '        End Using
    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Function

    Public Class Coord
        Public Property lon As Double
        Public Property lat As Double
    End Class

    Public Class City
        Public Property id As Integer
        Public Property name As String
        Public Property coord As Coord
        Public Property country As String
        Public Property population As Integer
    End Class

    Public Class Temp
        Public Property day As Double
        Public Property min As Double
        Public Property max As Double
        Public Property night As Double
        Public Property eve As Double
        Public Property morn As Double
    End Class

    Public Class Weather
        Public Property id As Integer
        Public Property main As String
        Public Property description As String
        Public Property icon As String
    End Class

    Public Class List
        Public Property dt As Integer
        Public Property temp As Temp
        Public Property pressure As Double
        Public Property humidity As Integer
        Public Property weather As List(Of Weather)
        Public Property speed As Double
        Public Property deg As Integer
        Public Property clouds As Integer
    End Class

    Public Class Previsao
        Public Property city As City
        Public Property cod As String
        Public Property message As Double
        Public Property cnt As Integer
        Public Property list As List(Of List)
    End Class

End Class

'' PEGA PREVISAO DO CLIMA
Public Class PrevisaoClima
    Shared appIdMinha As String = ""
    Shared appIdMinha2 As String = ""
    Shared appIdWeb As String = ""

    Shared baseUrl As String = "http://api.openweathermap.org/data/2.5/find/city?lat={0}&lon={1}&cnt={2}"

    Public Shared Function PrevisaoClimaCidade(ByVal nome As String) As Clima
        Dim client = New WebClient()
        client.Encoding = Encoding.UTF8
        client.Headers.Add("User-Agent", "Nobody")

        Dim url As String = String.Format("http://api.openweathermap.org/data/2.5/weather?q={0}&units=metric&cnt=1&APPID={1}&lang=pt", nome.Trim(), appIdWeb)

        Try
            Dim response = client.DownloadString(New Uri(url))
            'Dim previsao As WeatherInfo = JsonConvert.DeserializeObject(Of WeatherInfo)(response)
            Dim previsaoClima As Clima = JsonConvert.DeserializeObject(Of Clima)(response)
            Return previsaoClima
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    Public Class Coord
        Public Property lon As Double
        Public Property lat As Double
    End Class

    Public Class Weather
        Public Property id As Integer
        Public Property main As String
        Public Property description As String
        Public Property icon As String
    End Class

    Public Class Main
        Public Property temp As Double
        Public Property pressure As Double
        Public Property humidity As Integer
        Public Property temp_min As Double
        Public Property temp_max As Double
        Public Property sea_level As Double
        Public Property grnd_level As Double
    End Class

    Public Class Wind
        Public Property speed As Double
        Public Property deg As Double
    End Class

    Public Class Clouds
        Public Property all As Integer
    End Class

    Public Class Sys
        Public Property message As Double
        Public Property country As String
        Public Property sunrise As Integer
        Public Property sunset As Integer
    End Class

    Public Class Clima
        Public Property coord As Coord
        Public Property weather As List(Of Weather)
        Public Property base As String
        Public Property main As Main
        Public Property wind As Wind
        Public Property clouds As Clouds
        Public Property dt As Integer
        Public Property sys As Sys
        Public Property id As Integer
        Public Property name As String
        Public Property cod As Integer
    End Class

End Class

'' PEGA COTACAO DO DOLAR
Public Class CotacaoDolar
    'http://api.promasters.net.br/cotacao/v1/valores
    'https://olinda.bcb.gov.br/olinda/servico/PTAX/versao/v1/odata/CotacaoDolarDia(dataCotacao=@dataCotacao)?%40dataCotacao='06-06-2018'&%24format=json
    Public Shared Function valorDolar(ByVal data As String) As Dolar

        Dim client = New WebClient()
        client.Encoding = Encoding.UTF8
        client.Headers.Add("User-Agent", "Nobody")

        Dim url As String = String.Format("https://olinda.bcb.gov.br/olinda/servico/PTAX/versao/v1/odata/CotacaoDolarDia(dataCotacao=@dataCotacao)?%40dataCotacao='{0}'&%24format=json",
                                          data)
        '"06-06-2018")

        Try
            Dim response = client.DownloadString(New Uri(url))
            'Dim previsao As WeatherInfo = JsonConvert.DeserializeObject(Of WeatherInfo)(response)
            Dim cotacaoDolar As Dolar = JsonConvert.DeserializeObject(Of Dolar)(response)
            Return cotacaoDolar
        Catch ex As Exception
            Throw ex
        End Try

    End Function

    Public Class Value
        Public Property cotacaoCompra As Double
        Public Property cotacaoVenda As Double
        Public Property dataHoraCotacao As String
    End Class

    Public Class Dolar
        Public Property context As String
        Public Property value As List(Of Value)
    End Class

End Class

Public Class Correios

    Public Shared Function pesquisarCEP(ByVal CEP As String)

    End Function

    Public Shared Function pesquisarLogradouro(ByVal logradouro As String)

    End Function

End Class
