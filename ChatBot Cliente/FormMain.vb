Imports System.Net.Sockets

Public Class FormMain

    Dim clientSocket As New TcpClient()
    Dim ligado As Boolean = False

    '' BOTAO CONECTAR
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If ligado Then
            Button1.Text = "Conectar"
            ligado = False
            Button2.Enabled = False
            clientSocket.Close()
            AddLog("## Cliente Desconectado.")
        Else
            AddLog("## Tentando conectar...")
            Try
                'clientSocket.SendBufferSize = 65536
                'clientSocket.ReceiveBufferSize = 65536
                'clientSocket.SendTimeout = 5000
                'clientSocket.ReceiveTimeout = 5000

                'clientSocket.Connect("127.0.0.1", 147)
                clientSocket.Connect(TextBox1.Text, NumericUpDown1.Value)

                AddLog("## Conectado ao servidor!")
                Button1.Text = "Desconectar"
                ligado = True
                Button2.Enabled = True
                TabControl1.SelectedTab = TabControl1.TabPages.Item(1)
                TextBox2.Focus()

            Catch ex As Exception
                MsgBox("Não foi possível efetuar a conexão na porta indicada.")
                AddLog("## Não foi possivel conectar:")
                AddLog("## " & ex.Message)
            End Try
        End If

    End Sub

    '' ADD LOG
    Private Sub AddLog(ByVal linha As String)
        RichTextBox1.AppendText(linha)
        RichTextBox1.AppendText(vbNewLine)

        RichTextBox1.SelectionStart = RichTextBox1.TextLength
        RichTextBox1.ScrollToCaret()
    End Sub

    '' BOTAO ENVIAR ~> Receber
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If TextBox2.Text.Length > 1 Then
            Dim serverStream As NetworkStream = clientSocket.GetStream()

            Dim buffSize As Integer
            Dim outStream As Byte() = System.Text.Encoding.ASCII.GetBytes(TextBox2.Text & "$")
            AddLog("Cliente: " & TextBox2.Text)

            TextBox2.Clear()
            Dim cronometro As New Stopwatch

            Try
                cronometro.Start()

                serverStream.Write(outStream, 0, outStream.Length)
                serverStream.Flush()

                Dim inStream(65536) As Byte
                buffSize = clientSocket.ReceiveBufferSize
                serverStream.Read(inStream, 0, buffSize)


                'Dim returndata As String = System.Text.Encoding.ASCII.GetString(inStream)
                Dim returndata As String = System.Text.Encoding.UTF8.GetString(inStream)
                AddLog("Servidor: " & returndata)

                cronometro.Stop()
                ToolStripStatusLabel2.Text = "Tempo de resposta: " & cronometro.ElapsedMilliseconds.ToString & "ms"

            Catch ex As Exception
                MsgBox(ex.Message)
            End Try

        End If
    End Sub

    '' AO DIGITAR nas mensagens:
    Private Sub TextBox2_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox2.KeyDown
        If e.KeyData = Keys.Enter Then
            Button2.PerformClick()
        End If

    End Sub

    '' AO INICIAR
    Private Sub FormMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim strHostName = System.Net.Dns.GetHostName()
        'TextBox1.Text = System.Net.Dns.Resolve(strHostName).AddressList(0).ToString()
    End Sub

End Class
