/**
 * Kategori model arayüzü
 */
export interface Category {
  id: number;
  name: string;
  description: string;
  eventCount: number;
  createdAt: Date;
}