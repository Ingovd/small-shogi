module Shogi where

import Data.Array
import Data.Maybe

class Node a where
  children :: a -> [a]
  isTerminal :: a -> Maybe Bool
  
type Board = Array Integer (Array Integer (Maybe Piece))
pBoard :: Board -> String
pBoard b = concat [pColumn (b ! x) ++ "\n" | x <- [0..3]]
               where pColumn c = concat [pPiece (c ! y) | y <- [0..3]]
type Hand = [Role]
data Config = C Board Hand Board Hand Bool

data Role = K | G | S | N | L | P | B | R 
data Piece = Un Role | Pr Role
pPiece :: Maybe Piece -> String
pPiece Nothing = "   "
pPiece (Just (Un K)) = " K "
pPiece (Just (Un P)) = " P "

instance Node Config where
  children conf@(C wp wh bp bh b) = concat [genMove (wp ! x ! y) (x, y) conf | x <- [0..3], y <- [0..3]]
  isTerminal = undefined
  
type Coord = (Integer, Integer)
  
genMove :: Maybe Piece -> Coord -> Config -> [Config]
genMove Nothing _ _ = []
genMove (Just (Un K)) pos con = catMaybes [move pos con (x, y) | x <- [-1, 1], y <- [-1,1]]
genMove (Just (Un P)) pos con@(C _ _ _ _ b) = maybeToList $ move pos con (g b *  1, 0) 

g :: Bool -> Integer
g b = if b then 1 else -1

move :: Coord -> Config -> Coord -> Maybe Config
move = undefined

efile = array (0, 3) [(0, Just(Un P)), (1, Nothing), (2, Nothing), (3, Nothing)]
board = array (0, 3) [(0, efile), (1, efile), (2, efile), (3, efile)]