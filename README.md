# CrosswordCreator
Generates a crossword pattern from an array of words,

The key points/steps involved:

- When 2 words have atleast a single letter in common, they can be arranged in a 'T' shape with the intersecting letter as common. When more than 1 common intersecting letter is found, the first letter of the horizontal word is used.
An example of a T:
The Horizantal word is - badminton
The Vertical word id - cricket

    c
    r
badminton
    c
    k
    e
    t

- Arrange the words next to each other with one word in common so the incoming T already has a letter in common with the  developing puzzle in the grid.
As an example let's say we have the following list
{ "hockey", "football", "badminton", "cricket", "volleyball", "tennis", "billiards" }

It can be arranged in pair with one connecting word in common.
"hockey,football" (can become individual T)
"football,badminton" (can become individual T)
"badminton,cricket" (can become individual T)
"cricket,volleyball" (can become individual T)
"volleyball,tennis" (can become individual T)
"tennis,billiards" (can become individual T)

- The puzzle is dynamically created by attaching a new T to an existing puzzle in progress, till all the words in the comma separated string is done.

- If a T is detected but no spots are available then -- for the other word which do not yet exist on the grid, try attaching it to any of the existing words by detecting which letter is in common and again depending on spot-availability. If still no intersection is detected with any of the existing words, just place it at the bottom of the puzzle in the anticipation that the any new incoming words may pair up with this word.

- If no T is detected between the incoming pair of words in the current cycle, the logic tries to place the individual word (horizontal/vertical word, one at a time) with rest of the words existing so far in the grid.

- Repeat the above step for all rest of the words.

- Haven't extensively tested yet.
