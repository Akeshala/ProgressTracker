namespace ProgressTracker.Utils;

public static class ResultsLib
{
    public static(string grade, double studyRatio) GetStudyRatio(TimeSpan studied, TimeSpan target)
    {
        if (studied > target)
        {
            throw new ArgumentException("The studied date must be before the target date.");
        }

        var studyRatio = studied.TotalSeconds / target.TotalSeconds;

        var grade = studyRatio switch
        {
            >= 0.85 => "A+",
            >= 0.75 => "A",
            >= 0.70 => "A-",
            >= 0.65 => "B+",
            >= 0.60 => "B",
            >= 0.55 => "B-",
            >= 0.50 => "C+",
            >= 0.45 => "C",
            >= 0.40 => "C-",
            >= 0.35 => "D",
            _ => "F",
        };
        return (grade, studyRatio);
    }
}