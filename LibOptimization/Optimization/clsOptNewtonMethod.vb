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
        Private ReadOnly MAX_ITERATION As Integer = 1000
        Private ReadOnly EPS As Double = 0.00000001
        Private ReadOnly ALPHA As Double = 1.0

        'This parameters to use when generate a variable
        Private ReadOnly INIT_PARAM_RANGE As Double = 5

        'vector
        Private m_vect As clsShoddyVector = Nothing

        'ErrorManage
        Private m_error As New clsError
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_func">Optimize Function</param>
        ''' <param name="ai_randomRange">Optional:random range(Default 5 => -5 to 5)</param>
        ''' <param name="ai_maxIteration">Optional:Iteration(default 100)</param>
        ''' <param name="ai_eps">Optional:Eps(default:1e-8)</param>
        ''' <param name="ai_alpha">Optinal:update alpha(default 1.0)</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction, _
                       Optional ByVal ai_randomRange As Double = 5, _
                       Optional ByVal ai_maxIteration As Integer = 100, _
                       Optional ByVal ai_eps As Double = 0.00000001, _
                       Optional ByVal ai_alpha As Double = 1.0 _
                       )
            Me.m_func = ai_func
            Me.INIT_PARAM_RANGE = ai_randomRange
            Me.MAX_ITERATION = ai_maxIteration
            Me.EPS = ai_eps
            Me.ALPHA = ai_alpha

            Me.m_vect = New clsShoddyVector(ai_func.NumberOfVariable)
        End Sub
#End Region

#Region "Public"
        ''' <summary>
        ''' Init
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub Init()
            Try
                'Init value
                For i As Integer = 0 To Me.m_func.NumberOfVariable - 1
                    Me.m_vect(i) = Math.Abs(2.0 * INIT_PARAM_RANGE) * m_rand.NextDouble() - INIT_PARAM_RANGE
                Next
                Me.m_vect.Direction = clsShoddyVector.VectorDirection.COL
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
                Me.m_vect.Direction = clsShoddyVector.VectorDirection.COL
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
            Dim grad As New clsShoddyVector(MyBase.m_func.NumberOfVariable, clsShoddyVector.VectorDirection.COL)
            Dim h As New clsShoddyMatrix()
            ai_iteration = If(ai_iteration = 0, Me.MAX_ITERATION - 1, ai_iteration - 1)
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
                If MAX_ITERATION <= m_iteration Then
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