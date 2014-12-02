''' <summary>
''' Abstarct optimization Class
''' </summary>
''' <remarks></remarks>
Public MustInherit Class absOptimization
    Protected m_func As absObjectiveFunction = Nothing
    Protected m_iteration As Integer = 0

    ''' <summary>
    ''' Initialize parameter
    ''' </summary>
    ''' <remarks></remarks>
    Public MustOverride Sub Init()

    ''' <summary>
    ''' Do Iteration
    ''' </summary>
    ''' <param name="ai_iteration">Iteration count. When you set zero, use the default value.</param>
    ''' <returns>true:Stopping Criterion. false:Do not Stopping Criterion</returns>
    ''' <remarks></remarks>
    Public MustOverride Function DoIteration(Optional ByVal ai_iteration As Integer = 0) As Boolean

    ''' <summary>
    ''' Result
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public MustOverride ReadOnly Property Result() As clsPoint

    ''' <summary>
    ''' IsError
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public MustOverride Function IsRecentError() As Boolean

    ''' <summary>
    ''' Iteration count 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetIterationCount() As Integer
        Return Me.m_iteration
    End Function

    ''' <summary>
    ''' Get FunctionClass
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFunc() As absObjectiveFunction
        Return Me.m_func
    End Function
End Class
