Imports System.IO
Imports System.Data.SQLite






Public Class frmMain

    'Created by Peter Lawrow, 2016
    'We are updating two tables in Plex sqlite database: media_parts and sections
    'After we connect to database we import id and file fields and display file path in ListView.
    'In order to move database to a new location we need to substitute part of the file path and change forward or back slash in string.
    'Changes are displayed in preview ListView
    'For update media_parts table information is taken directly from preview ListView. Sections table is updated separately.


    Dim plexconnect As New SQLite.SQLiteConnection()
    Dim plexCommand As New SQLite.SQLiteCommand



    Private plexMedia As New DataTable
    Private plexSections As New DataTable

    Dim plexMediaAdapter As SQLiteDataAdapter
    Dim plexSectionAdapter As SQLiteDataAdapter






    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Text = "PlexDbFix"
    End Sub

    Private Sub btnOpenDbLocation_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOpenDbLocation.Click




        openFile()



    End Sub


    Private Sub openFile()



        Dim myStream As Stream = Nothing
        Dim openFileDialog1 As New OpenFileDialog()

        openFileDialog1.InitialDirectory = "c:\"
        openFileDialog1.Filter = "db files (*.db)|*.db|All files (*.*)|*.*"
        openFileDialog1.FilterIndex = 2
        openFileDialog1.RestoreDirectory = True



        If openFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            Try
                myStream = openFileDialog1.OpenFile()
                If (myStream IsNot Nothing) Then
                    ' Insert code to read the stream here.
                    txtDbLocation.Text = openFileDialog1.FileName




                End If
            Catch Ex As Exception
                MessageBox.Show("Cannot read file from disk. Original error: " & Ex.Message)
            Finally
                ' Check this again, since we need to make sure we didn't throw an exception on open.
                If (myStream IsNot Nothing) Then
                    myStream.Close()
                End If
            End Try
        End If





    End Sub

    Private Sub connectToPlexDB()


        Dim path As String = txtDbLocation.Text
        plexMedia.Clear()
        plexSections.Clear()


        plexconnect.ConnectionString = "Data Source=" & path & ";"

        plexconnect.Open()


        plexMediaAdapter = New SQLiteDataAdapter("SELECT * FROM media_parts", plexconnect)
        plexSectionAdapter = New SQLiteDataAdapter("SELECT * FROM section_locations", plexconnect)

        plexMediaAdapter.Fill(plexMedia)
        plexSectionAdapter.Fill(plexSections)

        plexconnect.Close()








    End Sub


    Private Sub fillListViewBeforeChanges()

        lvFileLocation.Items.Clear()



        Dim mediaFiles = From row In plexMedia.Rows.Cast(Of DataRow)()
                    Select New With {.idMovie = row("id"), .filepath = row("file")}

        Dim arrayMediaFiles As New ArrayList



        For Each Item In mediaFiles

            Dim lvItem As New ListViewItem

            lvItem.Text = Item.filepath



            arrayMediaFiles.Add(lvItem)

        Next




        lvFileLocation.Items.AddRange(DirectCast(arrayMediaFiles.ToArray(GetType(ListViewItem)), ListViewItem()))
        lvFileLocation.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.HeaderSize)





    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Try



            connectToPlexDB()
            fillListViewBeforeChanges()

        Catch ex As Exception

            MessageBox.Show("Database File Locaction cannot be empty")

        End Try


    End Sub





    Private Sub previewMediaPartsChanges()


        lvPreview.Items.Clear()
        lvPreview.View = View.Details



        Dim arrayPreview As New ArrayList
        Dim moveDbTo As String = cboMoveTo.Text.ToUpper.Trim


        Dim mediaParts = From row In plexMedia.Rows.Cast(Of DataRow)()
         Select New With {.idMedia = row("id"), .filepath = row("file")}


        For Each item In mediaParts

            Dim filePath As String = item.filepath.ToString



            Dim lvItem As New ListViewItem


            If filePath.Contains(txtOldLocation.Text) Then


                Dim editedPath As String = filePath.Replace(txtOldLocation.Text, txtNewLocation.Text)


                Select Case True


                    Case moveDbTo.ToString = "SERVER TO PC"


                        lvItem.Text = editedPath.Replace("/", "\")


                    Case moveDbTo.ToString = "PC TO SERVER"


                        lvItem.Text = editedPath.Replace("\", "/")



                    Case moveDbTo.ToString = "PC TO PC"


                        lvItem.Text = editedPath.Replace("\", "\")



                    Case moveDbTo.ToString = "SERVER TO SERVER"


                        lvItem.Text = editedPath.Replace("/", "/")



                End Select



                lvItem.Tag = item.idMedia
                arrayPreview.Add(lvItem)


            End If



        Next


        lvPreview.Items.AddRange(DirectCast(arrayPreview.ToArray(GetType(ListViewItem)), ListViewItem()))
        lvPreview.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.HeaderSize)




    End Sub




    Private Sub updateMediaParts()



        Dim path As String = txtDbLocation.Text

        plexconnect.ConnectionString = "Data Source=" & path & ";"

        plexconnect.Open()


        Dim transUpdateMediaParts As SQLiteTransaction
        Dim commandUpdateMediaParts As New SQLiteCommand

        transUpdateMediaParts = plexconnect.BeginTransaction
        commandUpdateMediaParts.Transaction = transUpdateMediaParts
        commandUpdateMediaParts.CommandText = "UPDATE media_parts SET file = @file WHERE id = @id"


        Dim mediaParts = From m In lvPreview.Items.Cast(Of ListViewItem)()
                  Select New With {.idMedia = m.Tag, .filePath = m.Text}




        For Each item In mediaParts

            commandUpdateMediaParts.Parameters.AddWithValue("@id", item.idMedia)
            commandUpdateMediaParts.Parameters.AddWithValue("@file", item.filePath)
            commandUpdateMediaParts.ExecuteNonQuery()
            commandUpdateMediaParts.Parameters.Clear()


        Next


        transUpdateMediaParts.Commit()
        transUpdateMediaParts.Dispose()

        plexconnect.Close()


    End Sub


    Private Sub updateSections()

        Dim path As String = txtDbLocation.Text

        plexconnect.ConnectionString = "Data Source=" & path & ";"

        plexconnect.Open()


        Dim transUpdateSections As SQLiteTransaction
        Dim commandUpdateSections As New SQLiteCommand
        Dim moveDbTo As String = cboMoveTo.Text.ToUpper.Trim
        Dim result As String = Nothing


        Dim sections = From row In plexSections.Rows.Cast(Of DataRow)()
                        Select New With {.id = row("id"), .filepath = row("root_path")}


        transUpdateSections = plexconnect.BeginTransaction
        commandUpdateSections.Transaction = transUpdateSections
        commandUpdateSections.CommandText = "UPDATE section_locations SET root_path = @root_path WHERE id = @id"




        For Each item In sections


            Dim filePath As String = item.filepath.ToString


            If filePath.Contains(txtOldLocation.Text) Then


                Dim editedPath As String = filePath.Replace(txtOldLocation.Text, txtNewLocation.Text)


                Select Case True


                    Case moveDbTo.ToString = "SERVER TO PC"


                        result = editedPath.Replace("/", "\")


                    Case moveDbTo.ToString = "PC TO SERVER"


                        result = editedPath.Replace("\", "/")



                    Case moveDbTo.ToString = "PC TO PC"


                        result = editedPath.Replace("\", "\")



                    Case moveDbTo.ToString = "SERVER TO SERVER"


                        result = editedPath.Replace("/", "/")



                End Select


                commandUpdateSections.Parameters.AddWithValue("@id", item.id)
                commandUpdateSections.Parameters.AddWithValue("@root_path", result)
                commandUpdateSections.ExecuteNonQuery()
                commandUpdateSections.Parameters.Clear()

            End If


        Next


        transUpdateSections.Commit()
        transUpdateSections.Dispose()



        plexconnect.Close()








    End Sub





    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click

        Try




            previewMediaPartsChanges()

        Catch ex As Exception

            MessageBox.Show("Unable to generate preview. Please ensure that ""Move database from/to"", ""Original File Location"" and ""New File Location"" are not empty. ")

        End Try



    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click

        Try

            updateMediaParts()
            updateSections()


            MessageBox.Show("Job Completed")


        Catch ex As Exception

            MessageBox.Show("Unable to apply changes: " & ex.Message)

        End Try

    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click

        openFile()

    End Sub

    Private Sub ConnectToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ConnectToolStripMenuItem.Click

        Try
            connectToPlexDB()
            fillListViewBeforeChanges()

        Catch ex As Exception

            MessageBox.Show("Unable to connect to database: " & ex.Message)

        End Try

    End Sub

    Private Sub ExitToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem1.Click

        Me.Close()

    End Sub

    Private Sub ReloadToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReloadToolStripMenuItem.Click



        Try

            connectToPlexDB()
            fillListViewBeforeChanges()


        Catch ex As Exception

            MessageBox.Show("Unable to connect to database: " & ex.Message)

        End Try

    End Sub

    Private Sub ResetToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResetToolStripMenuItem.Click

        cboMoveTo.Text = Nothing
        txtOldLocation.Text = Nothing
        txtNewLocation.Text = Nothing
        lvPreview.Items.Clear()

    End Sub

    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click

        frmAbout.Show()
    End Sub
End Class
