const getNumberOfCorrectMatches = (selectedAnswers: number[]) => {
    const correctAnswers = selectedAnswers.filter((answer, i) => answer === i);
    
    return correctAnswers.length;
}

const areMostOfTheMatchesCorrect = (selectedAnswers: number[]) => {
    const totalQuestions = selectedAnswers.length;
    const totalCorrect = questionHelper.getNumberOfCorrectMatches(selectedAnswers);
    
    return totalCorrect / totalQuestions >= 0.5;
}

export const questionHelper = {
    getNumberOfCorrectMatches,
    areMostOfTheMatchesCorrect
};
