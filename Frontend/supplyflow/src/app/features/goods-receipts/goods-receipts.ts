import {
  ChangeDetectorRef,
  Component,
  OnInit,
} from '@angular/core';

import {
  CommonModule,
} from '@angular/common';

import {
  FormArray,
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';

import {
  GoodsReceiptService,
} from '../../core/services/goods-receipt.service';

import {
  GoodsReceipt,
  CreateGoodsReceipt,
  CreateGoodsReceiptItem,
} from '../../core/models/goods-receipt.model';

import {
  PurchaseOrderService,
} from '../../core/services/purchase-order.service';

import {
  PurchaseOrder,
  PurchaseOrderItem,
} from '../../core/models/purchase-order.model';


@Component({
  selector: 'app-goods-receipts',
  standalone: true,

  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
  ],

  templateUrl: './goods-receipts.html',

  styleUrl: './goods-receipts.css',
})
export class GoodsReceipts implements OnInit {

  goodsReceipts: GoodsReceipt[] = [];

  filteredGoodsReceipts: GoodsReceipt[] = [];

  purchaseOrders: PurchaseOrder[] = [];

  selectedPurchaseOrder:
    PurchaseOrder | null = null;

  selectedGoodsReceipt:
    GoodsReceipt | null = null;


  isLoading = false;

  isSubmitting = false;


  errorMessage = '';

  successMessage = '';


  searchText = '';

  statusFilter = 'All';


  showCreateModal = false;

  showViewModal = false;


  receiptForm: FormGroup;


  constructor(
    private fb: FormBuilder,

    private goodsReceiptService:
      GoodsReceiptService,

    private purchaseOrderService:
      PurchaseOrderService,

    private cdr:
      ChangeDetectorRef
  ) {

    this.receiptForm =
      this.fb.group({

        purchaseOrderId: [
          '',
          Validators.required,
        ],

        receiptDate: [
          this.getTodayDate(),
          Validators.required,
        ],

        remarks: [''],

        items:
          this.fb.array([]),
      });
  }


  ngOnInit(): void {

    this.loadGoodsReceipts();

    this.loadPurchaseOrders();
  }


  /* =====================================
     GET FORM ITEMS
  ===================================== */

  get items(): FormArray {

    return this.receiptForm.get(
      'items'
    ) as FormArray;
  }


  /* =====================================
     LOAD GOODS RECEIPTS
  ===================================== */

  loadGoodsReceipts(): void {

    this.isLoading = true;

    this.errorMessage = '';

    this.cdr.detectChanges();


    this.goodsReceiptService
      .getAll()
      .subscribe({

        next: (response) => {

          this.goodsReceipts =
            response ?? [];

          this.applyFilters();

          this.isLoading = false;

          this.cdr.detectChanges();
        },


        error: (error) => {

          console.error(
            'Failed to load goods receipts:',
            error
          );

          this.errorMessage =
            error?.error?.message ??
            'Failed to load goods receipts.';

          this.goodsReceipts = [];

          this.filteredGoodsReceipts = [];

          this.isLoading = false;

          this.cdr.detectChanges();
        },
      });
  }


  /* =====================================
     LOAD PURCHASE ORDERS
  ===================================== */

  loadPurchaseOrders(): void {

    this.purchaseOrderService
      .getAll()
      .subscribe({

        next: (response) => {

          this.purchaseOrders =
            response ?? [];

          this.cdr.detectChanges();
        },


        error: (error) => {

          console.error(
            'Failed to load purchase orders:',
            error
          );

          this.purchaseOrders = [];

          this.cdr.detectChanges();
        },
      });
  }


  /* =====================================
     PURCHASE ORDER CHANGE
  ===================================== */

  onPurchaseOrderChange(): void {

    const purchaseOrderId =
      Number(
        this.receiptForm.get(
          'purchaseOrderId'
        )?.value
      );


    if (!purchaseOrderId) {

      this.selectedPurchaseOrder =
        null;

      this.items.clear();

      this.cdr.detectChanges();

      return;
    }


    this.isLoading = true;

    this.cdr.detectChanges();


    this.purchaseOrderService
      .getById(
        purchaseOrderId
      )
      .subscribe({

        next: (purchaseOrder) => {

          this.selectedPurchaseOrder =
            purchaseOrder;

          this.populateItems(
            purchaseOrder.items ?? []
          );

          this.isLoading = false;

          this.cdr.detectChanges();
        },


        error: (error) => {

          console.error(
            'Failed to load purchase order details:',
            error
          );

          this.errorMessage =
            error?.error?.message ??
            'Failed to load purchase order details.';

          this.selectedPurchaseOrder =
            null;

          this.items.clear();

          this.isLoading = false;

          this.cdr.detectChanges();
        },
      });
  }


  /* =====================================
     POPULATE PURCHASE ORDER ITEMS
  ===================================== */

  populateItems(
    purchaseOrderItems:
      PurchaseOrderItem[]
  ): void {

    this.items.clear();


    purchaseOrderItems.forEach(
      (item) => {

        const remainingQuantity =
          Number(
            item.remainingQuantity ?? 0
          );


        if (remainingQuantity <= 0) {
          return;
        }


        this.items.push(
          this.fb.group({

            productId: [
              item.productId,
              Validators.required,
            ],

            purchaseOrderItemId: [
              item.id,
              Validators.required,
            ],

            productName: [
              item.productName,
            ],

            sku: [
              item.sku,
            ],

            orderedQuantity: [
              item.orderedQuantity,
            ],

            receivedQuantity: [
              remainingQuantity,
              [
                Validators.required,
                Validators.min(0.01),
              ],
            ],

            acceptedQuantity: [
              remainingQuantity,
              [
                Validators.required,
                Validators.min(0),
              ],
            ],

            rejectedQuantity: [
              0,
              [
                Validators.required,
                Validators.min(0),
              ],
            ],

            remarks: [''],
          })
        );
      }
    );


    this.cdr.detectChanges();
  }


  /* =====================================
     RECEIVED QUANTITY CHANGE
  ===================================== */

  updateQuantities(
    index: number
  ): void {

    const item =
      this.items.at(index);


    const received =
      Number(
        item.get(
          'receivedQuantity'
        )?.value
      ) || 0;


    let accepted =
      Number(
        item.get(
          'acceptedQuantity'
        )?.value
      ) || 0;


    if (accepted > received) {

      accepted = received;

      item.get(
        'acceptedQuantity'
      )?.setValue(
        accepted,
        {
          emitEvent: false,
        }
      );
    }


    const rejected =
      Math.max(
        0,
        received - accepted
      );


    item.get(
      'rejectedQuantity'
    )?.setValue(
      rejected,
      {
        emitEvent: false,
      }
    );


    this.cdr.detectChanges();
  }


  /* =====================================
     ACCEPTED QUANTITY CHANGE
  ===================================== */

  onAcceptedQuantityChange(
    index: number
  ): void {

    const item =
      this.items.at(index);


    const received =
      Number(
        item.get(
          'receivedQuantity'
        )?.value
      ) || 0;


    let accepted =
      Number(
        item.get(
          'acceptedQuantity'
        )?.value
      ) || 0;


    if (accepted > received) {

      accepted = received;

      item.get(
        'acceptedQuantity'
      )?.setValue(
        accepted,
        {
          emitEvent: false,
        }
      );
    }


    if (accepted < 0) {

      accepted = 0;

      item.get(
        'acceptedQuantity'
      )?.setValue(
        accepted,
        {
          emitEvent: false,
        }
      );
    }


    const rejected =
      Math.max(
        0,
        received - accepted
      );


    item.get(
      'rejectedQuantity'
    )?.setValue(
      rejected,
      {
        emitEvent: false,
      }
    );


    this.cdr.detectChanges();
  }


  /* =====================================
     OPEN CREATE MODAL
  ===================================== */

  openCreateModal(): void {

    this.errorMessage = '';

    this.successMessage = '';


    this.selectedPurchaseOrder =
      null;


    this.items.clear();


    this.receiptForm.reset({

      purchaseOrderId: '',

      receiptDate:
        this.getTodayDate(),

      remarks: '',
    });


    this.showCreateModal = true;

    this.cdr.detectChanges();
  }


  /* =====================================
     CLOSE CREATE MODAL
  ===================================== */

  closeCreateModal(): void {

    this.showCreateModal = false;


    this.selectedPurchaseOrder =
      null;


    this.items.clear();


    this.receiptForm.reset({

      purchaseOrderId: '',

      receiptDate:
        this.getTodayDate(),

      remarks: '',
    });


    this.cdr.detectChanges();
  }


  /* =====================================
     SUBMIT GOODS RECEIPT
  ===================================== */

  submitGoodsReceipt(): void {

    this.errorMessage = '';

    this.successMessage = '';


    if (
      this.receiptForm.invalid
    ) {

      this.receiptForm
        .markAllAsTouched();


      this.errorMessage =
        'Please fill all required fields.';


      this.cdr.detectChanges();

      return;
    }


    if (
      this.items.length === 0
    ) {

      this.errorMessage =
        'Please select a purchase order with remaining items.';


      this.cdr.detectChanges();

      return;
    }


    const request:
      CreateGoodsReceipt = {

        purchaseOrderId:
          Number(
            this.receiptForm.value
              .purchaseOrderId
          ),

        receiptDate:
          this.receiptForm.value
            .receiptDate,

        remarks:
          this.receiptForm.value
            .remarks || null,

        items:
          this.items.controls.map(
            (control) => {

              const value =
                control.value;


              const item:
                CreateGoodsReceiptItem = {

                  productId:
                    Number(
                      value.productId
                    ),

                  purchaseOrderItemId:
                    Number(
                      value.purchaseOrderItemId
                    ),

                  receivedQuantity:
                    Number(
                      value.receivedQuantity
                    ),

                  acceptedQuantity:
                    Number(
                      value.acceptedQuantity
                    ),

                  rejectedQuantity:
                    Number(
                      value.rejectedQuantity
                    ),

                  remarks:
                    value.remarks || null,
                };


              return item;
            }
          ),
      };


    this.isSubmitting = true;

    this.cdr.detectChanges();


    this.goodsReceiptService
      .create(request)
      .subscribe({

        next: () => {

          this.isSubmitting = false;


          this.closeCreateModal();


          this.successMessage =
            'Goods receipt created successfully.';


          this.loadGoodsReceipts();

          this.cdr.detectChanges();
        },


        error: (error) => {

          console.error(
            'Failed to create goods receipt:',
            error
          );


          this.errorMessage =
            error?.error?.message ??
            'Failed to create goods receipt.';


          this.isSubmitting = false;

          this.cdr.detectChanges();
        },
      });
  }


  /* =====================================
     VIEW GOODS RECEIPT
  ===================================== */

  viewGoodsReceipt(
    goodsReceipt:
      GoodsReceipt
  ): void {

    this.errorMessage = '';


    this.goodsReceiptService
      .getById(
        goodsReceipt.id
      )
      .subscribe({

        next: (response) => {

          this.selectedGoodsReceipt =
            response;


          this.showViewModal = true;

          this.cdr.detectChanges();
        },


        error: (error) => {

          console.error(
            'Failed to load goods receipt:',
            error
          );


          this.errorMessage =
            error?.error?.message ??
            'Failed to load goods receipt details.';


          this.cdr.detectChanges();
        },
      });
  }


  /* =====================================
     CLOSE VIEW MODAL
  ===================================== */

  closeViewModal(): void {

    this.showViewModal = false;

    this.selectedGoodsReceipt =
      null;

    this.cdr.detectChanges();
  }


  /* =====================================
     COMPLETE GOODS RECEIPT
  ===================================== */

  completeGoodsReceipt(
    goodsReceipt:
      GoodsReceipt
  ): void {

    const confirmed =
      confirm(
        `Complete GRN ${goodsReceipt.grnNumber}?`
      );


    if (!confirmed) {
      return;
    }


    this.errorMessage = '';

    this.successMessage = '';


    this.goodsReceiptService
      .complete(
        goodsReceipt.id
      )
      .subscribe({

        next: () => {

          this.successMessage =
            'Goods receipt completed successfully.';


          this.loadGoodsReceipts();

          this.cdr.detectChanges();
        },


        error: (error) => {

          console.error(
            'Failed to complete goods receipt:',
            error
          );


          this.errorMessage =
            error?.error?.message ??
            'Failed to complete goods receipt.';


          this.cdr.detectChanges();
        },
      });
  }


  /* =====================================
     CANCEL GOODS RECEIPT
  ===================================== */

  cancelGoodsReceipt(
    goodsReceipt:
      GoodsReceipt
  ): void {

    const confirmed =
      confirm(
        `Cancel GRN ${goodsReceipt.grnNumber}?`
      );


    if (!confirmed) {
      return;
    }


    this.errorMessage = '';

    this.successMessage = '';


    this.goodsReceiptService
      .cancel(
        goodsReceipt.id
      )
      .subscribe({

        next: () => {

          this.successMessage =
            'Goods receipt cancelled successfully.';


          this.loadGoodsReceipts();

          this.cdr.detectChanges();
        },


        error: (error) => {

          console.error(
            'Failed to cancel goods receipt:',
            error
          );


          this.errorMessage =
            error?.error?.message ??
            'Failed to cancel goods receipt.';


          this.cdr.detectChanges();
        },
      });
  }


  /* =====================================
     SEARCH CHANGE
  ===================================== */

  onSearchChange(): void {

    this.applyFilters();
  }


  /* =====================================
     STATUS FILTER CHANGE
  ===================================== */

  onStatusFilterChange(): void {

    this.applyFilters();
  }


  /* =====================================
     APPLY FILTERS
  ===================================== */

  applyFilters(): void {

    const search =
      this.searchText
        .toLowerCase()
        .trim();


    this.filteredGoodsReceipts =
      this.goodsReceipts.filter(
        (goodsReceipt) => {

          const grnNumber =
            (
              goodsReceipt.grnNumber ??
              ''
            ).toLowerCase();


          const purchaseOrderNumber =
            (
              goodsReceipt.purchaseOrderNumber ??
              ''
            ).toLowerCase();


          const warehouseName =
            (
              goodsReceipt.warehouseName ??
              ''
            ).toLowerCase();


          const receivedBy =
            (
              goodsReceipt.receivedByUserName ??
              ''
            ).toLowerCase();


          const matchesSearch =
            !search ||

            grnNumber.includes(
              search
            ) ||

            purchaseOrderNumber.includes(
              search
            ) ||

            warehouseName.includes(
              search
            ) ||

            receivedBy.includes(
              search
            );


          const matchesStatus =

            this.statusFilter ===
              'All' ||

            !this.statusFilter ||

            goodsReceipt.status ===
              this.statusFilter;


          return (
            matchesSearch &&
            matchesStatus
          );
        }
      );


    this.cdr.detectChanges();
  }


  /* =====================================
     STATUS CLASS
  ===================================== */

  getStatusClass(
    status: string
  ): string {

    switch (
      (
        status ?? ''
      ).toLowerCase()
    ) {

      case 'draft':

        return 'bg-yellow-100 text-yellow-700';


      case 'completed':

        return 'bg-green-100 text-green-700';


      case 'cancelled':

        return  'bg-red-100 text-red-700';


      default:

        return 'bg-slate-100 text-slate-700';
    }
  }


  /* =====================================
     GET TODAY DATE
  ===================================== */

  private getTodayDate():
    string {

    const today =
      new Date();


    const year =
      today.getFullYear();


    const month =
      String(
        today.getMonth() + 1
      ).padStart(
        2,
        '0'
      );


    const day =
      String(
        today.getDate()
      ).padStart(
        2,
        '0'
      );


    return `${year}-${month}-${day}`;
  }
}
