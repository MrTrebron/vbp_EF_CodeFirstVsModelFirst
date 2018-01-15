Imports CodeFirst_Model.Interfaces

Public Class Blog
    Inherits ModelBase
    Implements ILogicalDelete
    Implements ILogicalTimestamp

    Public Sub New()
    End Sub

    Public Sub New(title As String, posts As List(Of Post))
        Me.Title = title : Me.Posts = posts
    End Sub

    Public Overridable Property Id As Integer
    Public Overridable Property Title As String
    Public Overridable Property Description As string

    Public Overridable Property Posts As ICollection(Of Post)

    Public Property DeletedFlag As Boolean Implements ILogicalDelete.DeletedFlag
    Public Property DeletedTimestamp As Date? Implements ILogicalDelete.DeletedTimestamp
    Public Property LastUpdateTimestamp As Date? Implements ILogicalTimestamp.LastUpdateTimestamp
    Public Property CreationTimestamp As Date? Implements ILogicalTimestamp.CreationTimestamp


    Public Sub AddPost(p As Post)
        Posts.Add(p)
    End Sub




    Public Overrides Function ToString() As String
        Return _
            $"*** Blog-Objekt: {Title} - {If(Posts Is Nothing, "0", Posts.Count.ToString) _
                } Posts in Blog vorhanden. Letztes Update am {LastUpdateTimestamp.Value.ToLongDateString} durch { _
                LastUpdateBy}. Gelöscht: {DeletedFlag.ToString}"
    End Function

End Class
