Imports LibOptimization.Util
Imports LibOptimization.MathTool

Namespace Optimization
    ''' <summary>
    ''' Steepest descent method / gradient method
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Use first derivertive.
    '''  -First order conversion.
    ''' 
    ''' Refference:
    ''' [1]http://dsl4.eee.u-ryukyu.ac.jp/DOCS/nlp/node4.html
    ''' [2]金谷健一, "これならわかる最適化数学－基礎原理から計算手法まで－", 共立出版株式会社 2007 初版第7刷, pp79-84 
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' 
    ''' Memo:
    ''' 最適化で微分を用いて求める手法のことを「勾配法」という。
    ''' 最大値を求めることを山登り法、最小値の場合は最急降下法とよばれる。
    ''' </remarks>
    <Serializable>
    Public Class clsOptSteepestDescent : Inherits absOptimization
#Region "Member"
        ''' <summary>Max iteration count</summary>
        Public Overrides Property Iteration As Integer = 10000

        ''' <summary>Epsilon(Default:1e-8) for Criterion</summary>
        Public Property EPS As Double = 0.00000001

        '-------------------------------------------------------------------
        'Coefficient of SteepestDescent
        '-------------------------------------------------------------------
        ''' <summary>rate</summary>
        Public Property ALPHA As Double = 0.3

        'vector
        Private m_vect As DenseVector = Nothing
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_func">Optimize Function</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction)
            Me.m_func = ai_func

            Me.m_vect = New DenseVector(ai_func.NumberOfVariable)
        End Sub
#End Region

#Region "Public"
        ''' <summary>
        ''' Init
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub Init()
            Try
                ' メンバ変数の初期化
                Me.m_iteration = 0
                Me.m_vect.Clear()
                Me.m_error.Clear()

                ' InitialPosition のチェック
                If MyBase.InitialPosition IsNot Nothing Then
                    If MyBase.InitialPosition.Length <> MyBase.m_func.NumberOfVariable Then
                        Throw New ArgumentException("The number of variavles in InitialPosition and objective function are different.")
                    End If
                End If

                ' m_vect の初期化（集団に InitialPosition を含む）
                If MyBase.InitialPosition IsNot Nothing AndAlso MyBase.InitialPosition.Length = Me.m_func.NumberOfVariable Then
                    ' 指定された InitialPosition をそのまま利用する
                    Me.m_vect = New DenseVector(MyBase.InitialPosition)
                Else
                    ' InitialPosition が指定されていない場合は、乱数で生成した配列を利用する
                    Dim randArray() As Double = clsUtil.GenRandomPositionArray(Me.m_func, Nothing, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)
                    Me.m_vect = New DenseVector(randArray)
                End If

            Catch ex As Exception
                Me.m_error.SetError(True, clsError.ErrorType.ERR_INIT)
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
            Dim grad As New DenseVector(MyBase.m_func.NumberOfVariable)
            If Me.Iteration <= m_iteration Then
                Return True
            Else
                ai_iteration = If(ai_iteration = 0, Iteration - m_iteration - 1, Math.Min(ai_iteration, Iteration - m_iteration) - 1)
            End If
            For iterate As Integer = 0 To ai_iteration
                'Counting generation
                m_iteration += 1

                'Calculate Gradient vector
                grad.RawVector = Me.m_func.Gradient(Me.m_vect)

                'Update
                Me.m_vect = Me.m_vect - Me.ALPHA * grad

                'Check conversion
                If Me.IsUseCriterion = True Then
                    If grad.NormL2() < EPS Then
                        Return True
                    End If
                End If
            Next

            Return False
        End Function

        ''' <summary>
        ''' Result
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Result As clsPoint
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
                Dim ret As New List(Of clsPoint)
                ret.Add(New clsPoint(MyBase.m_func, Me.m_vect))
                Return ret
            End Get
        End Property
#End Region
    End Class

End Namespace