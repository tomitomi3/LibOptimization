Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Newton Method
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Use second derivertive.
    '''  -Second order conversion.
    ''' 
    ''' Refference:
    ''' 金谷健一, "これならわかる最適化数学－基礎原理から計算手法まで－", 共立出版株式会社 2007 初版第7刷, pp79-84 
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' 
    ''' Memo:
    ''' 最適化で微分を用いて求める手法のことを「勾配法」という。
    ''' 最大値を求めることを山登り法、最小値の場合は最急降下法とよばれる。
    ''' </remarks>
    Public Class clsOptNewtonMethod : Inherits absOptimization
#Region "Member"
        ''' <summary>Max iteration count(Default:5,000)</summary>
        Public Property Iteration As Integer = 5000

        ''' <summary>Epsilon(Default:1e-8) for Criterion</summary>
        Public Property EPS As Double = 0.00000001

        ''' <summary>hessian matrix coefficient(Default:1.0)</summary>
        Public Property ALPHA As Double = 1.0

        'vector
        Private m_vect As clsEasyVector = Nothing
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_func">Optimize Function</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction)
            Me.m_func = ai_func

            Me.m_vect = New clsEasyVector(ai_func.NumberOfVariable)
        End Sub
#End Region

#Region "Public"
        ''' <summary>
        ''' Init
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub Init()
            Try
                'init meber varibles
                Me.m_iteration = 0
                Me.m_vect.Clear()
                Me.m_error.Clear()

                'Init value
                For i As Integer = 0 To Me.m_func.NumberOfVariable - 1
                    Dim value As Double = clsUtil.GenRandomRange(Me.m_rand, -Me.InitialValueRange, Me.InitialValueRange)
                    Me.m_vect.Add(value)
                Next
                Me.m_vect.Direction = clsEasyVector.VectorDirection.COL

                'add initial position
                If InitialPosition IsNot Nothing AndAlso InitialPosition.Length = m_func.NumberOfVariable Then
                    Me.m_vect = New clsEasyVector(InitialPosition, clsEasyVector.VectorDirection.COL)
                End If
            Catch ex As Exception
                Me.m_error.SetError(True, clsError.ErrorType.ERR_INIT)
            End Try
        End Sub

        ''' <summary>
        ''' Init
        ''' </summary>
        ''' <remarks></remarks>
        Public Overloads Sub Init(ByVal ai_initPoint() As Double)
            Try
                'init meber varibles
                If ai_initPoint.Length <> Me.m_func.NumberOfVariable Then
                    Me.m_error.SetError(True, clsError.ErrorType.ERR_INIT, "")
                    Return
                End If

                Me.m_vect = ai_initPoint
                Me.m_vect.Direction = clsEasyVector.VectorDirection.COL
            Catch ex As Exception
                Me.m_error.SetError(True, clsError.ErrorType.ERR_INIT, "")
            Finally
                System.GC.Collect()
            End Try
        End Sub

        ''' <summary>
        ''' Do Iteration
        ''' </summary>
        ''' <param name="ai_iteration">Iteration count. When you set zero, use the default value.</param>
        ''' <returns>True:Stopping Criterion. False:Do not Stopping Criterion</returns>
        ''' <remarks></remarks>
        Public Overrides Function DoIteration(Optional ByVal ai_iteration As Integer = 0) As Boolean
            'Check Last Error
            If Me.IsRecentError() = True Then
                Return True
            End If

            'Do Iterate
            Dim grad As New clsEasyVector(MyBase.m_func.NumberOfVariable, clsEasyVector.VectorDirection.COL)
            Dim h As New clsEasyMatrix()
            ai_iteration = If(ai_iteration = 0, Me.Iteration - 1, ai_iteration - 1)
            For iterate As Integer = 0 To ai_iteration
                'Calculate Gradient vector
                grad.RawVector = Me.m_func.Gradient(Me.m_vect)

                'Calculate Hessian matrix
                h.RawMatrix = Me.m_func.Hessian(Me.m_vect)

                'Update
                Me.m_vect = Me.m_vect - ALPHA * h.Inverse() * grad 'H^-1 calulate heavy...

                'Check conversion
                If grad.NormL1() < EPS Then
                    Return True
                End If

                'Iteration cuont
                If Iteration <= m_iteration Then
                    Me.m_error.SetError(True, clsError.ErrorType.ERR_OPT_MAXITERATION)
                    Return True
                End If
                m_iteration += 1
            Next

            Return False
        End Function

        ''' <summary>
        ''' Result
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Result() As clsPoint
            Get
                Return New clsPoint(MyBase.m_func, Me.m_vect)
            End Get
        End Property

        ''' <summary>
        ''' Result for debug.(not implementation)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' for Debug
        ''' </remarks>
        Public Overrides ReadOnly Property Results As List(Of clsPoint)
            Get
                Return Nothing
            End Get
        End Property

        ''' <summary>
        ''' Get recent error infomation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetLastErrorInfomation() As clsError.clsErrorInfomation
            Return Me.m_error.GetLastErrorInfomation()
        End Function

        ''' <summary>
        ''' Get recent error
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function IsRecentError() As Boolean
            Return Me.m_error.IsError()
        End Function
#End Region
    End Class

End Namespace