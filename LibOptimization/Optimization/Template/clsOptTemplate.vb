Imports LibOptimization.Util
Imports LibOptimization.MathTool

Namespace Optimization
    ''' <summary>
    ''' optimize template not use
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable>
    Public Class clsOptTemplate : Inherits absOptimization
#Region "Member"
        '----------------------------------------------------------------
        'Common parameters
        '----------------------------------------------------------------
        ''' <summary>
        ''' epsilon(Default:1e-8) for Criterion
        ''' </summary>
        Public Property EPS As Double = 0.000000001

        ''' <summary>
        ''' higher N percentage particles are finished at the time of same evaluate value.
        ''' This parameter is valid is when IsUseCriterion is true.
        ''' </summary>
        Public Property HigherNPercent As Double = 0.8 'for IsCriterion()
        Private HigherNPercentIndex As Integer = 0 'for IsCriterion())

        ''' <summary>Max Iteration(Default:10,000)</summary>
        Public Overrides Property Iteration As Integer = 10000

        '----------------------------------------------------------------
        'Peculiar parameter
        '----------------------------------------------------------------
        ''' <summary>
        ''' Population Size(Default:100)
        ''' </summary>
        Public Property PopulationSize As Integer = 100

        'population
        Private m_populations As New List(Of clsPoint)
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_func">Objective Function</param>
        ''' <remarks>
        ''' </remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction)
            Me.m_func = ai_func
        End Sub
#End Region

#Region "Public"
        ''' <summary>
        ''' Init
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub Init()

            '-----------------------------------------
            Throw New Exception("not use this class. this class is template.")
            '-----------------------------------------

            Try
                'init varibles
                Me.m_iteration = 0
                Me.m_populations.Clear()
                Me.m_error.Clear()

                'check initialposition
                If MyBase.InitialPosition IsNot Nothing Then
                    If MyBase.InitialPosition.Length = MyBase.m_func.NumberOfVariable Then
                        'nothing
                    Else
                        Throw New ArgumentException("The number of variavles in InitialPosition and objective function are different.")
                    End If
                End If

                'initial position
                For i As Integer = 0 To Me.PopulationSize - 1
                    Dim array = clsUtil.GenRandomPositionArray(Me.m_func, InitialPosition, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)
                    Me.m_populations.Add(New clsPoint(Me.m_func, array))
                Next

                'Sort Evaluate
                Me.m_populations.Sort()

                'Detect HigherNPercentIndex
                Me.HigherNPercentIndex = CInt(Me.m_populations.Count * Me.HigherNPercent)
                If Me.HigherNPercentIndex = Me.m_populations.Count OrElse Me.HigherNPercentIndex >= Me.m_populations.Count Then
                    Me.HigherNPercentIndex = Me.m_populations.Count - 1
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
            If Me.Iteration <= m_iteration Then
                Return True
            Else
                ai_iteration = If(ai_iteration = 0, Iteration - m_iteration - 1, Math.Min(ai_iteration, Iteration - m_iteration) - 1)
            End If
            For iterate As Integer = 0 To ai_iteration
                'Counting generation
                m_iteration += 1

                'Sort Evaluate
                Me.m_populations.Sort()

                'check criterion
                If Me.IsUseCriterion = True Then
                    'higher N percentage particles are finished at the time of same evaluate value.
                    If clsUtil.IsCriterion(Me.EPS, Me.m_populations(0).Eval, Me.m_populations(Me.HigherNPercentIndex).Eval) Then
                        Return True
                    End If
                End If

                'add logic
            Next

            Return False
        End Function

        ''' <summary>
        ''' Best result
        ''' </summary>
        ''' <returns>Best point class</returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Result As clsPoint
            Get
                Return Me.m_populations(0)
            End Get
        End Property

        ''' <summary>
        ''' All Result
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' for Debug
        ''' </remarks>
        Public Overrides ReadOnly Property Results As List(Of clsPoint)
            Get
                Return Me.m_populations
            End Get
        End Property
#End Region

#Region "Private"

#End Region
    End Class
End Namespace
