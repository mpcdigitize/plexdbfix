Public Class frmAbout

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Me.Close()
    End Sub

    

    Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Try
            VisitLink()
        Catch ex As Exception
            ' The error message
            MessageBox.Show("Unable to open link that was clicked.")
        End Try

    End Sub

    Sub VisitLink()
        ' Change the color of the link text by setting LinkVisited 
        ' to True.
        LinkLabel1.LinkVisited = True
        ' Call the Process.Start method to open the default browser 
        ' with a URL:
        System.Diagnostics.Process.Start("http://mpcdigitize.com/")
    End Sub

    Private Sub frmAbout_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.AutoSizeMode = False


     
    End Sub

  

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
       
        System.Diagnostics.Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=EZ6HYRBS7DY4U&lc=US&item_name=PlexDbFix&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donate_LG%2egif%3aNonHosted")

    End Sub
End Class