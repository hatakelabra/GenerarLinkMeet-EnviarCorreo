Imports System.Data.SqlClient
Imports System.Threading
Imports System.Configuration
Imports System.IO
Imports System.Net.Mail

Module Funciones

    Public coneccionHosp As String = "Data Source=****;Initial Catalog=***;Persist Security Info=True;User ID=***;Password=*****"
    Public coneccionHospTest As String = "Data Source=*****;Initial Catalog=***;Persist Security Info=True;User ID=***;Password=***"
    Public link, rut, citacion, correo, medico, nombrePaciente, hora As String
    Public correoOrigen As String = ""
    Public contrasena As String = ""
    Public ex, ey As Integer
    Public Arrastre As Boolean

    Public Function ConexionBD(ByVal NomConexion As String, ByVal _Sql As String) As DataTable

        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand
        Dim dt As New DataTable
        Dim adapter As New SqlDataAdapter

        Try

            conn.ConnectionString = NomConexion
            cmd.CommandType = CommandType.Text
            cmd.CommandText = _Sql
            cmd.Connection = conn
            adapter.SelectCommand = cmd
            adapter.Fill(dt)

            Return dt

        Catch ex As Exception
            Return dt
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
                conn.Dispose()
            End If
        End Try
    End Function

    Public Function TraeDatos()
        Dim consulta As String = "SELECT top(20) *
                                  FROM **** WHERE ****"
        'InputBox("", "", consulta)
        Dim t As New DataTable
        t = ConexionBD(coneccionHospTest, consulta)
        Return t
    End Function

    Public Function GuardaLog(fecha As String, run As String, enlace As String, mail As String, id As String)
        Dim mensaje As String = ""
        Dim consulta As String = ""

        Dim myconnect As New SqlConnection

        myconnect.ConnectionString = coneccionHosp

        Dim mycommand As SqlCommand = New SqlCommand()
        mycommand.Connection = myconnect
        mycommand.CommandText = "INSERT INTO Log
                                    (FechaCreado, FechaCita, RutPaciente, LinkMeet, CorreoPaciente, idCitacionDetalle) 
                                VALUES 
                                    (@FechaCreado, @FechaCita, @RutPaciente, @LinkMeet, @CorreoPaciente, @idCitacionDetalle)"

        myconnect.Open()

        Try
            mycommand.Parameters.AddWithValue("@FechaCreado", DateTime.Now)
            mycommand.Parameters.AddWithValue("@FechaCita", fecha)
            mycommand.Parameters.AddWithValue("@RutPaciente", run)
            mycommand.Parameters.AddWithValue("@LinkMeet", enlace)
            mycommand.Parameters.AddWithValue("@CorreoPaciente", mail)
            mycommand.Parameters.AddWithValue("@idCitacionDetalle", id)

            mycommand.ExecuteNonQuery()
            'ExecuteScalar
            mensaje = "Success"
        Catch ex As SqlException
            mensaje = ex.Message
        End Try
        myconnect.Close()
        Return mensaje

    End Function

    Public Function EnviarCorreo(mensaje As String, mail As String)

        Dim _Message As New System.Net.Mail.MailMessage()
        Dim _SMTP As New System.Net.Mail.SmtpClient
        'Dim att As New System.Net.Mail.Attachment(TextAdjunto.Text) ', System.Net.Mime.TransferEncoding.Base64  //activar si se requiere adjuntar archivo
        'CONFIGURACIÓN DEL STMP
        _SMTP.Credentials = New System.Net.NetworkCredential(correoOrigen, contrasena)
        _SMTP.Host = "smtp.gmail.com"
        _SMTP.Port = 25
        _SMTP.EnableSsl = True

        ' CONFIGURACION DEL MENSAJE
        _Message.[To].Add(mail) 'Para quién se lo envio.
        _Message.From = New System.Net.Mail.MailAddress(correoOrigen, "Titulo a mostrar en correo", System.Text.Encoding.UTF8) 'Quien lo envía
        _Message.Subject = "Asunto"
        _Message.SubjectEncoding = System.Text.Encoding.UTF8 'Codificacion
        _Message.Body = mensaje
        _Message.BodyEncoding = System.Text.Encoding.UTF8
        _Message.Priority = System.Net.Mail.MailPriority.Normal
        ' _Message.Attachments.Add(att) //activar si se requiere adjuntar archivo
        _Message.IsBodyHtml = True

        'ENVIO
        Try
            _SMTP.Send(_Message)
        Catch ex As Exception
            MsgBox(ex.Message.ToString)
        End Try

        Return "success"
    End Function

End Module
