Imports CodeFirst_Model.Interfaces

Public Class Post
    Inherits ModelBase
    Implements ILogicalDelete
    Implements ILogicalTimestamp

    Public Sub New()
        Title = "Untitled"
    End Sub

    Public Sub New(title As String, content As String)
        Me.Title = title : Me.Content = content
    End Sub

    Public Sub New(title As String, content As String, openedBy As User, markedUsers As List(Of User))
        Me.Title = title : Me.Content = content : Me.OpenedBy = openedBy : Me.MarkedUsers = markedUsers
    End Sub


    Public Overridable Property Id As Integer
    Public Overridable Property Title As String
    Public Overridable Property Content As String

    Public Overridable Property OpenedByUserId As Integer
    Public Overridable Property OpenedBy As User

    Public Overridable Property MarkedUsers As ICollection(Of User)

    Public Overridable Property BlogId As Integer
    Public Overridable Property Blog As Blog


    Public Property DeletedFlag As Boolean Implements ILogicalDelete.DeletedFlag
    Public Property DeletedTimestamp As Date? Implements ILogicalDelete.DeletedTimestamp
    Public Property LastUpdateTimestamp As Date? Implements ILogicalTimestamp.LastUpdateTimestamp
    Public Property CreationTimestamp As Date? Implements ILogicalTimestamp.CreationTimestamp

    Public Overrides Function ToString() As String
        Return String.Format("Post-Objekt: ID/Title: {1} - {2} (gespeichert in Blog '{3}'){0}   Content:{4}{0}   Eröffnet durch: {5}{0}   Gelöscht: {6} - Letzte Änderung durch '{7}' am {8}",
                             Environment.NewLine, Id, Title, Blog.Title, Content, OpenedBy.UserName, DeletedFlag, If(LastUpdateBy Is Nothing, "Niemanden", LastUpdateBy), If(LastUpdateTimestamp.HasValue, LastUpdateTimestamp.Value.ToLongDateString, "Nie"))
    End Function

End Class
