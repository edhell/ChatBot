Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading

Public Class FormMain

    'Private portaChat As Integer = 147
    Private thread1 As Thread
    Public Shared ligado As Boolean = False
    Public Shared contador As Integer = 0

    '' AO INICIAR
    Private Sub FormMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If My.Computer.Network.IsAvailable Then
            'Dim strHostName = System.Net.Dns.GetHostName()
            'Dim strIPAddress = System.Net.Dns.Resolve(strHostName).AddressList(0).ToString()

            Dim ipLocal As System.Net.IPAddress = GetLocalIP()

            TextBox1.Text = ipLocal.ToString
            'TextBox1.Text = "127.0.0.1"

            'Button1.PerformClick()

        Else
            MsgBox("Esta desconectado!?")
        End If

    End Sub

    '' FUNCAO PEGAR IP
    Function GetLocalIP() As System.Net.IPAddress
        Dim localIP() As System.Net.IPAddress = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName)
        For Each current As System.Net.IPAddress In localIP
            If current.ToString Like "*[.]*[.]*[.]*" Then
                Try
                    Dim SplitVar() As String = current.ToString.Split(".")
                    If Len(SplitVar(0)) <= 3 And Len(SplitVar(1)) <= 3 And Len(SplitVar(2)) <= 3 And Len(SplitVar(3)) <= 3 Then
                        Return current
                    End If
                Catch ex As Exception
                End Try
            End If
        Next
    End Function

    '' BOTAO Iniciar Servidor
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If ligado Then
            ligado = False
            Button1.Text = "Ligar"
            AddLog("## Servidor Desligado")
            'If clientSocket.Connected Then : clientSocket.Close() : End If
            'If serverSocket.Server.Connected Then : serverSocket.Stop() : End If
            'thread1.Suspend()

        Else
            AddLog("## Ligando Servidor ChatBot")
            ligado = True
            Button1.Text = "Desligar"
            Button1.Enabled = False

            'Dim enderecoLocal As IPAddress = IPAddress.Parse("127.0.0.1")
            Dim enderecoLocal As IPAddress = IPAddress.Parse(TextBox1.Text)

            Dim servidorSocket As New TcpListener(enderecoLocal, NumericUpDown1.Value)
            'serverSocket.Server.SendBufferSize = 65536
            'serverSocket.Server.ReceiveBufferSize = 65536
            'serverSocket.Server.SendTimeout = 5000
            'serverSocket.Server.ReceiveTimeout = 5000

            thread1 = New Thread(AddressOf iniciarServidor)
            thread1.Name = "Thread Servidor"
            thread1.Start(servidorSocket)

        End If

    End Sub

    '' THREAD 1 Iniciar Servidor
    Private Sub iniciarServidor(ByVal serverSocket As TcpListener)

        'Dim serverSocket As TcpListener = Nothing

        Dim clientSocket As TcpClient = Nothing
        serverSocket.Start()

        Me.Invoke(Sub() AddLog("## Servidor inicializado. Aguardando requisições..."))
        Me.Invoke(Sub() ToolStripStatusLabel2.Text = "Servidor ONLINE")

        While (ligado)
            clientSocket = serverSocket.AcceptTcpClient()

            'contador += 1
            Me.Invoke(Sub() contador += 1)
            Me.Invoke(Sub() AddLog("## Cliente " + Convert.ToString(contador) + " iniciado!"))
            Me.Invoke(Sub() ToolStripStatusLabel4.Text = contador & " Cliente(s)")

            Me.Invoke(Sub() iniciarConexaoCliente(clientSocket, contador))

        End While

        Me.Invoke(Sub() AddLog("## Servidor desligado!"))
        clientSocket.Close()
        serverSocket.Stop()

    End Sub

    '' FUNCAO DA THREAD
    Private Sub iniciarConexaoCliente(ByVal clienteSocket As TcpClient, ByVal numeroCliente As String)
        Dim cliente As New trataCliente
        cliente.iniciarCliente(clienteSocket, Convert.ToString(contador))

    End Sub

    '' CLASSE DO CLIENTE
    Public Class trataCliente
        Dim clientSocket As TcpClient
        Dim clNo As String

        '' INICIA CLIENTE
        Public Sub iniciarCliente(ByVal inClientSocket As TcpClient, ByVal clineNo As String)
            Me.clientSocket = inClientSocket
            Me.clNo = clineNo

            Dim ctThread As Threading.Thread = New Threading.Thread(AddressOf doChat)

            ctThread.Start()
        End Sub

        '' ADD LOG
        Private Sub addlogMain(ByVal linha As String)
            FormMain.AddLog(linha)
            'MsgBox(linha)
        End Sub

        '' RECEBE E ENVIA DADOS
        Private Sub doChat()
            Dim requestCount As Integer
            Dim bytesFrom(65536) As Byte
            Dim dataFromClient As String
            Dim sendBytes As [Byte]()
            Dim serverResponse As String
            Dim cidade As String = "Santa Cruz do Sul"

            requestCount = 0

            While (FormMain.ligado)
                Try
                    requestCount = requestCount + 1
                    Dim networkStream As NetworkStream = clientSocket.GetStream()
                    networkStream.Read(bytesFrom, 0, CInt(clientSocket.ReceiveBufferSize))

                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom)
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"))

                    '' Recebeu dados:
                    addlogMain("Do cliente(" & clNo & "): " & dataFromClient)

                    '' Se for função: (começando com "/")
                    If (dataFromClient.StartsWith("/") And dataFromClient.Length > 1) Then
                        serverResponse = "C(" + clNo + ") Req(" & requestCount & "): "

                        '' Comando recebido:
                        Dim comandoTemp As String = dataFromClient.Replace("/", "")
                        comandoTemp = comandoTemp.ToLower.Trim
                        Dim comando() As String = comandoTemp.Split(" ")
                        Dim parametro As String = comandoTemp.Replace(comando(0), "").Trim

                        '' Comandos definidos:
                        Try
                            Select Case comando(0)
                                Case "servidor", "server"
                                    Dim pc As PerformanceCounter = New PerformanceCounter("System", "System Up Time")
                                    pc.NextValue() ' Retorna 0

                                    Dim cpu As New System.Diagnostics.PerformanceCounter()
                                    With cpu
                                        .CategoryName = "Processor"
                                        .CounterName = "% Processor Time"
                                        .InstanceName = "_Total"
                                    End With
                                    cpu.NextValue() ' Retorna 0

                                    Dim ts As TimeSpan = TimeSpan.FromSeconds(pc.NextValue()) ' Pega o valor exato.
                                    Dim memoriaLivre As Double = My.Computer.Info.AvailablePhysicalMemory
                                    memoriaLivre = memoriaLivre / (1024 * 1024)

                                    serverResponse &= " Servidor: " & My.Computer.Name & vbNewLine
                                    serverResponse &= " * Sistema: " & My.Computer.Info.OSFullName & vbNewLine
                                    serverResponse &= " * Tempo ligado: " & ts.Days & "D " & ts.Hours & ":" & ts.Minutes & ":" & ts.Seconds & "" & vbNewLine
                                    serverResponse &= " * Memória livre: " & (Math.Round(memoriaLivre, 2)).ToString & "MB" & vbNewLine
                                    serverResponse &= " * Uso de CPU: " & cpu.NextValue() & "%" & vbNewLine
                                    'serverResponse &= "Usuários: " &  & vbNewLine
                                    serverResponse &= " * Serviços Online: Todos serviços online." & vbNewLine

                                Case "ajuda", "help"
                                    serverResponse &= "Funções: " & vbNewLine
                                    serverResponse &= " * /donos: " & vbNewLine
                                    serverResponse &= " * /dataHora, /data, /hora: " & vbNewLine
                                    serverResponse &= " * /temperatura , /temp, /tempo: " & vbNewLine
                                    serverResponse &= " * /randNoticia, /noticia, /news: " & vbNewLine
                                    serverResponse &= " * /cotacaoDolar, /dolar: " & vbNewLine
                                    serverResponse &= " * /pesquisarCEP: " & vbNewLine
                                    serverResponse &= " * /salvarCidade " & vbNewLine
                                    serverResponse &= " * /pesquisarLogradouro, /pesquisaRua, /pesquisaAv: " & vbNewLine
                                    serverResponse &= " * /servidor, /server: " & vbNewLine
                                    serverResponse &= " * /ajuda , /help: " & vbNewLine
                                    'serverResponse &= " * /exit, /shutdown, /sair, /desligar, /desconectar: " & vbNewLine

                                Case "donos"
                                    serverResponse &= "Desenvolvedores: " & vbNewLine
                                    serverResponse &= " * Eduardo Dumke " & vbNewLine
                                    serverResponse &= " * Mateus Witz " & vbNewLine

                                Case "datahora", "data", "hora"
                                    serverResponse &= "Data Hora: " & Now()

                                Case "temperatura", "temp", "tempo"
                                    serverResponse &= "Previsão do Tempo: " & vbNewLine

                                    'Dim weatherInfo As WeatherInfo
                                    Dim TempoInfo As New PrevisaoTempo.Previsao
                                    Dim ClimaInfo As New PrevisaoClima.Clima

                                    If comando.Length > 1 Then

                                        'weatherInfo = PrevisaoTempo.PrevisaoTempoCidade(parametro)
                                        TempoInfo = PrevisaoTempo.PrevisaoTempoCidade(parametro)
                                        ClimaInfo = PrevisaoClima.PrevisaoClimaCidade(parametro)

                                    Else
                                        'TempoInfo = PrevisaoTempo.PrevisaoTempoCidade("Santa Cruz do Sul")
                                        'ClimaInfo = PrevisaoClima.PrevisaoClimaCidade("Santa Cruz do Sul")
                                        TempoInfo = PrevisaoTempo.PrevisaoTempoCidade(cidade)
                                        ClimaInfo = PrevisaoClima.PrevisaoClimaCidade(cidade)
                                    End If

                                    Try
                                        'Dim weatherInfo As WeatherInfo = PrevisaoTempo.PrevisaoTempoCidade("Santa Cruz do Sul, RS")

                                        serverResponse &= " * Cidade: " & TempoInfo.city.name & ", " & TempoInfo.city.country & vbNewLine
                                        serverResponse &= " * População: " & TempoInfo.city.population & " Habitantes" & vbNewLine
                                        serverResponse &= " * Descrição: " & TempoInfo.list(0).weather(0).description & vbNewLine
                                        serverResponse &= " * Temperatura Atual: " & String.Format("{0}°С", Math.Round(ClimaInfo.main.temp, 1)) & vbNewLine
                                        serverResponse &= " * Temperatura Min: " & String.Format("{0}°С", Math.Round(TempoInfo.list(0).temp.min, 1)) & vbNewLine
                                        serverResponse &= " * Temperatura Min: " & String.Format("{0}°С", Math.Round(TempoInfo.list(0).temp.max, 1)) & vbNewLine
                                        serverResponse &= " * Vento: " & String.Format("{0}m/seg", Math.Round(ClimaInfo.wind.speed, 1)) & vbNewLine
                                        serverResponse &= " * Umidade: " & TempoInfo.list(0).humidity.ToString() & "%" & vbNewLine
                                        serverResponse &= " * Nuvens: " & String.Format("{0}%", ClimaInfo.clouds.all) & vbNewLine

                                    Catch ex As Exception
                                        serverResponse &= " Erro: " & ex.Message
                                    End Try

                                Case "randnoticia", "noticia", "noticias", "news"
                                    Dim noticias As New Noticia.Noticias

                                    If comando.Length > 1 Then
                                        noticias = Noticia.carregarNoticias(parametro)
                                    Else
                                        noticias = Noticia.carregarNoticias("")
                                    End If

                                    serverResponse &= noticias.totalResults & " Notícias: " & vbNewLine

                                    For Each n In noticias.articles
                                        serverResponse &= " * " & n.title & ": " & vbNewLine
                                        serverResponse &= "Descrição: " & n.description & vbNewLine
                                        serverResponse &= "- - - - - - - - - - - - - - - - - - - - - - - - - - - - -" & vbNewLine & vbNewLine
                                    Next

                                Case "cotacaodolar", "dolar"
                                    serverResponse &= "Dolar: " & vbNewLine
                                    Dim dolar As New CotacaoDolar.Dolar

                                    parametro = parametro.Trim
                                    parametro.Replace("/", "-")
                                    parametro.Replace(" ", "-")

                                    If String.IsNullOrEmpty(parametro.Trim) Then
                                        '' Data de ontem:
                                        'MsgBox(DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy"))
                                        dolar = CotacaoDolar.valorDolar(DateTime.Now.AddDays(-1).ToString("MM-dd-yyyy"))
                                    Else
                                        dolar = CotacaoDolar.valorDolar(parametro)
                                    End If

                                    serverResponse &= " * Compra: " & dolar.value(0).cotacaoCompra & ": " & vbNewLine
                                    serverResponse &= " * Venda: " & dolar.value(0).cotacaoVenda & ": " & vbNewLine
                                    serverResponse &= " * Data: " & dolar.value(0).dataHoraCotacao & ": " & vbNewLine

                                Case "pesquisacep"
                                    serverResponse &= " PESQUISA CEP: " & parametro & ": " & vbNewLine

                                    Try
                                        Dim ds1 As New DataSet
                                        Dim xml1 As String = "http://cep.republicavirtual.com.br/web_cep.php?cep=" & parametro & "&formato=xml"

                                        ds1.ReadXml(xml1)

                                        serverResponse &= " * " & ds1.Tables(0).Rows(0)("resultado_txt").ToString().ToUpper & vbNewLine
                                        serverResponse &= " * Logradouro: " & ds1.Tables(0).Rows(0)("tipo_logradouro").ToString() & " " & ds1.Tables(0).Rows(0)("logradouro").ToString() & vbNewLine
                                        serverResponse &= " * Bairro: " & ds1.Tables(0).Rows(0)("bairro").ToString() & vbNewLine
                                        serverResponse &= " * Cidade: " & ds1.Tables(0).Rows(0)("cidade").ToString() & vbNewLine
                                        serverResponse &= " * UF: " & ds1.Tables(0).Rows(0)("uf").ToString() & vbNewLine

                                    Catch ex As Exception
                                        serverResponse &= " Não encontrado. " & vbNewLine

                                    End Try

                                Case "cidade"

                                    serverResponse &= "Cidade modificada para: " & parametro & vbNewLine

                                    Try
                                        cidade = parametro.Trim
                                    Catch ex As Exception
                                        serverResponse &= "Erro ao modificar cidade padrão." & vbNewLine
                                    End Try

                                Case "pesquisalogradouro", "pesquisarua", "pesquisaav"
                                    serverResponse &= "Pesquisa Logradouro: " & parametro & vbNewLine

                                    Try
                                        'Dim xml1 As String = "http://viacep.com.br/ws/RS/santa%20cruz%20do%20sul/" & parametro & "/xml/"
                                        Dim xml1 As String = "http://viacep.com.br/ws/RS/" & cidade.Trim & "/" & parametro & "/xml/"

                                        Dim ds1 As New DataSet
                                        ds1.ReadXml(xml1)

                                        serverResponse &= " * Resultados: " & ds1.Tables(1).Rows.Count & vbNewLine

                                        For Each x In ds1.Tables(1).Rows
                                            serverResponse &= " * CEP: " & x("cep").ToString & vbNewLine
                                            serverResponse &= " * Logradouro: " & x("logradouro").ToString & vbNewLine
                                            serverResponse &= " * Bairro: " & x("bairro").ToString & vbNewLine
                                            serverResponse &= " * Complemento: " & x("complemento").ToString & vbNewLine

                                            serverResponse &= " -------------------------- " & vbNewLine
                                        Next

                                    Catch ex As Exception
                                        serverResponse &= "Erro ao procurar logradouro. " & vbNewLine
                                    End Try

                                Case "trendtwitter"
                                    serverResponse &= "Em desenvolvimento. " & vbNewLine

                                Case "rnddicionario"
                                    serverResponse &= "Em desenvolvimento. " & vbNewLine

                                Case "rndemoji"
                                    serverResponse &= "Em desenvolvimento. " & vbNewLine

                                Case "rndimg"
                                    serverResponse &= "Em desenvolvimento. " & vbNewLine

                                Case "exit", "shutdown", "sair", "desligar", "desconectar"
                                    serverResponse &= "Desconectando..." & vbNewLine
                                    serverResponse &= "Em desenvolvimento. " & vbNewLine
                                    'Exit While

                                Case Else
                                    serverResponse &= "SERVIÇO NÃO ENCONTRADO, tente: /Ajuda ou /help" & vbNewLine

                            End Select

                        Catch ex As Exception
                            serverResponse = "ERRO: " & ex.Message

                        End Try
                    Else
                        '' Se não é função, só retorna mensagem enviada:
                        serverResponse = "C(" + clNo + ") Req(" & requestCount & "): Texto: " & dataFromClient

                    End If

                    ''Envia Resposta:
                    'sendBytes = Encoding.ASCII.GetBytes(serverResponse)
                    sendBytes = Encoding.UTF8.GetBytes(serverResponse)

                    networkStream.Write(sendBytes, 0, sendBytes.Length)
                    networkStream.Flush()
                    FormMain.AddLog(serverResponse)

                Catch ex As Exception
                    'MsgBox(ex.ToString)

                    addlogMain("Cliente " & clNo & ": desligado!")
                    'Thread.CurrentThread.Abort()
                End Try

            End While

        End Sub

    End Class


    '' ADD LOG
    Public Sub AddLog(ByVal linha As String)
        RichTextBox1.AppendText(linha)
        RichTextBox1.AppendText(vbNewLine)

        RichTextBox1.SelectionStart = RichTextBox1.TextLength
        RichTextBox1.ScrollToCaret()
    End Sub

    '' AO FECHAR
    Private Sub FormMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Try
            ligado = False
            'thread1.Suspend()
            thread1.Abort()
        Catch ex As Exception : End Try

    End Sub

End Class