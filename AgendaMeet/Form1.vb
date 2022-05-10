Imports OpenQA.Selenium
Imports OpenQA.Selenium.Chrome

'https://www.automatetheplanet.com/selenium - webdriver - vbn - cheat - sheet / (LINK devariables Selenium)
'https://chromedriver.chromium.org/downloads ChromeDriver LINK
'Versión 101.0.4951.54 (Versión utilizada de Chrome)

Public Class Form1

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TimerInicio.Enabled = True
        TimerInicio.Start()
        TimerRutina.Enabled = True
        TimerRutina.Start()

    End Sub

    Public Async Sub AsignaLink()
        Dim resultado = TraeDatos()
        Dim err As DataRow() = resultado.Select("[id_citacion] > 0")
        Dim total As Integer = err.Count
        'Call barra(total)
        TimerInicio.Enabled = False
        ProgressBar1.Visible = True
        ' Set Minimum to 1 to represent the first file being copied.
        ProgressBar1.Minimum = 1
        ' Set Maximum to the total number of files to copy.
        ' Set the initial value of the ProgressBar.
        ProgressBar1.Value = 1
        ' Set the Step property to a value of 1 to represent each file being copied.
        ProgressBar1.Step = 1
        ProgressBar1.Maximum = total

        If err.Count > 0 Then
            For i As Integer = 0 To err.Count - 1
                rut = resultado.Rows(i).Item("run") & "-" & resultado.Rows(i).Item("dv")
                Dim run As String = resultado.Rows(i).Item("run")
                correo = resultado.Rows(i).Item("correo_telemedicina")
                nombrePaciente = resultado.Rows(i).Item("nombres") & " " & resultado.Rows(i).Item("paterno") & " " & resultado.Rows(i).Item("materno")
                citacion = resultado.Rows(i).Item("fecha")
                Dim idCita As String = resultado.Rows(i).Item("id_citacion_detalle")
                Dim prestacion As String = resultado.Rows(i).Item("nombre_prestacion")
                hora = resultado.Rows(i).Item("hora")
                Lb_MjeCta.Text = "Procesando  " & i & " de " & total
                ProgressBar1.PerformStep()
                Application.DoEvents()
                Me.Cursor = Cursors.Default

                Call Rutina()

                GuardaLog(citacion, run, link, correo, idCita)

                Dim mensaje As String = "Le recordamos que para realizar su atención debe ingresar al link entregado" &
                    " y conectarse con 15 minutos de anticipación a su consulta para verificar conexión y uso de la plataforma." &
                    " Recuerde que debe contar con cámara y micrófono para su cita."
                Dim mensajeCorreo As String = "<!doctype html>" &
                    "<html><head><style>*{font-family: Montserrat,sans-serif;margin: 0;padding: .5rem;color: #4a4a4a;}" &
                    "</style></head><body><img src=''>" &
                    "<div style ='width:550px;'><h1 style='color:#065F33; font-size: 1.5em; font-weight: 400;'>TELEMEDICINA</h1>" &
                    "<h3 style='color:#04337B; font-size: 1.25em; font-weight: 600'>ATENCIÓN FAVOR NO RESPONDER MENSAJE. </h3>" &
                    "<div style='height: 1px;height: 1px; background-color: #4a4a4a88; padding: 0;'></div> <p></p>" &
                    "<p> Estimado/a Paciente: " & nombrePaciente & ", le enviamos la información correspondiente a su próxima cita en Hospital DIPRECA. </p>" &
                    "<br>" &
                    "<p> Fecha de atención: " & citacion & " a las: " & hora & " </p>" &
                    "<p> Especialidad: " & prestacion & " </p>" &
                    "<p> Link para su consulta: " & link & " </p>" &
                    "<p>&nbsp;<b></b></p><br>" &
                    "<div style ='width:550px;'><p style='color:#065F33;'>" & mensaje & "</p>" &
                    "<p>&nbsp;<b></b></p><br>" &
                    "<p>Atentamente,</p>" &
                    "<p>NIÑITA</p>" &
                    "<img src='' style='max-width: 100%;'>" &
                    "</div></body></html>"


                EnviarCorreo(mensajeCorreo, correo)
                Await Task.Delay(120000)
            Next

        End If

    End Sub

    Public Sub Rutina()
        link = ""
        Dim driver As IWebDriver
        'Dim options As New ChromeOptions
        'options.AddArgument("--headless")
        'options.AddArgument("--disable-gpu")
        'Dim service As ChromeDriverService = ChromeDriverService.CreateDefaultService
        'service.HideCommandPromptWindow = True
        Dim service As ChromeDriverService = ChromeDriverService.CreateDefaultService()
        service.HideCommandPromptWindow = True

        Dim options = New ChromeOptions()
        options.AddArgument("--window-position=-32000,-32000")

        driver = New ChromeDriver(service, options)
        driver.Navigate().GoToUrl("https://meet.google.com/getalink?hs=202&authuser=0&illm=1651500782124&hl=es")
        driver.FindElement(By.Id("identifierId")).SendKeys(correoOrigen)
        driver.FindElement(By.ClassName("VfPpkd-vQzf8d")).Click()
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5)
        driver.FindElement(By.Name("password")).SendKeys(contrasena)
        driver.FindElement(By.Id("passwordNext")).Click()
        link = driver.FindElement(By.ClassName("oXn32")).Text()
        driver.Quit()

    End Sub

    Private Sub TimerInicio_Tick(sender As Object, e As EventArgs) Handles TimerInicio.Tick
        Call AsignaLink()
    End Sub

    Private Sub TimerRutina_Tick(sender As Object, e As EventArgs) Handles TimerRutina.Tick
        Call AsignaLink()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
    End Sub

    Private Sub Form1_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown
        ex = e.X
        ey = e.Y
        Arrastre = True
    End Sub

    Private Sub Form1_MouseUp(sender As Object, e As MouseEventArgs) Handles MyBase.MouseUp
        If Arrastre Then Me.Location = Me.PointToScreen(New Point(Me.MousePosition.X - Me.Location.X - ex, Me.MousePosition.Y - Me.Location.Y - ey))
    End Sub

End Class

