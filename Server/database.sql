-- Add new columns to rooms table
ALTER TABLE rooms
ADD COLUMN status ENUM('waiting', 'in_progress', 'completed') DEFAULT 'waiting',
ADD COLUMN current_item_index INT DEFAULT 0,
ADD COLUMN auction_start_time DATETIME DEFAULT NULL;

-- Create auction_history table to track bids
CREATE TABLE IF NOT EXISTS auction_history (
    id INT AUTO_INCREMENT PRIMARY KEY,
    room_id INT NOT NULL,
    item_id INT NOT NULL,
    bidder_id INT NOT NULL,
    bid_amount DOUBLE NOT NULL,
    bid_time DATETIME NOT NULL,
    FOREIGN KEY (room_id) REFERENCES rooms(id) ON DELETE CASCADE,
    FOREIGN KEY (bidder_id) REFERENCES accounts(id) ON DELETE CASCADE
); 